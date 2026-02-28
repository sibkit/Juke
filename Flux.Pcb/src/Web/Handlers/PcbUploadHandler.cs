/* PcbUploadHandler.cs */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using BoardFlow.Converters.GerberToSgm;
using BoardFlow.Formats.Excellon.Reading;
using BoardFlow.Formats.Gerber.Reading;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Svg.Entities;
using BoardFlow.Formats.Svg.Writing;
using Flux.Pcb.Data;
using Juke.ServiceLocation;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;
using Path = System.IO.Path; 

namespace Flux.Pcb.Web.Handlers;

public class PcbUploadHandler : FileUploadHandler
{
    protected override string GetTargetDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "flux_pcb_uploads");
        if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
        return tempDir;
    }

    protected override async Task OnFilesSavedAsync(IHttpContext context, IReadOnlyList<SavedFileInfo> savedFiles)
    {
        if (savedFiles.Count == 0)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Файл не загружен.");
            return;
        }

        var archive = savedFiles[0];
        if (!archive.PhysicalPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Ожидается ZIP архив.");
            return;
        }

        var orderId = Guid.NewGuid();
        var extractPath = Path.Combine(Path.GetTempPath(), "flux_pcb_unzip", orderId.ToString());
        var svgOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Orders", orderId.ToString());

        Directory.CreateDirectory(extractPath);
        Directory.CreateDirectory(svgOutputDir);

        // СОХРАНЕНИЕ ОРИГИНАЛЬНОГО АРХИВА для последующего доступа к raw-файлам
        var permanentZipPath = Path.Combine(svgOutputDir, "archive.zip");
        File.Copy(archive.PhysicalPath, permanentZipPath, true);

        try
        {
            ZipFile.ExtractToDirectory(archive.PhysicalPath, extractPath, overwriteFiles: true);
            var extractedFiles = new DirectoryInfo(extractPath).GetFiles("*.*", SearchOption.AllDirectories);
            
            var parsedDocuments = new List<(string FileName, SvgDocument SvgDoc)>();
            var failedFiles = new List<string>(); 
            var ignoredFiles = new List<string>(); // Для пропущенных
            var globalGerberBounds = Bounds.Empty();
            var drillFiles = new List<FileInfo>();
            
            foreach (var fileInfo in extractedFiles)
            {
                var ext = fileInfo.Extension.ToUpperInvariant();
                
                if (fileInfo.Name.StartsWith(".")) continue;

                if (ext is ".DRL" or ".TXT" or ".TAP") 
                {
                    drillFiles.Add(fileInfo); 
                    continue;
                }

                // Документация и картинки добавляем в игнорируемые
                if (ext is ".TC" or ".TOL" or ".PNG" or ".PDF" or ".JPG" or ".JPEG" or ".DOC" or ".DOCX" or ".XML" or ".CSV")
                {
                    ignoredFiles.Add(fileInfo.Name);
                    continue;
                }

                try {
                    var readResult = GerberReader.Instance.Read(fileInfo);
                    if (readResult.Item1.Operations.Count > 0) {
                        var svgDoc = GerberToSpvConverter.Convert(readResult.Item1);
                        parsedDocuments.Add((fileInfo.Name, svgDoc));
                        var localBounds = svgDoc.ViewBox ?? SvgWriter.CalculateViewBox(svgDoc);
                        if (localBounds.MinX != double.PositiveInfinity) globalGerberBounds = globalGerberBounds.ExtendBounds(localBounds);
                    } else {
                        failedFiles.Add(fileInfo.Name + " (Пустой файл)");
                    }
                } catch (Exception ex) { 
                    failedFiles.Add(fileInfo.Name + $" ({ex.Message})"); 
                }
            }

            double gerberWidth = globalGerberBounds.MinX != double.PositiveInfinity ? globalGerberBounds.GetWidth() : 0;

            foreach (var drillFile in drillFiles)
            {
                try {
                    var readResult = ExcellonReader.Instance.Read(drillFile);
                    if (readResult.Item1.Operations.Count > 0)
                    {
                        double minX = double.MaxValue, maxX = double.MinValue;
                        foreach (var op in readResult.Item1.Operations) {
                            if (op is BoardFlow.Formats.Excellon.Entities.DrillOperation d) {
                                if (d.StartPoint.X < minX) minX = d.StartPoint.X;
                                if (d.StartPoint.X > maxX) maxX = d.StartPoint.X;
                            }
                        }
                        double drillWidth = maxX != double.MinValue ? (maxX - minX) : 0;

                        double scale = 1.0;
                        if (gerberWidth > 10 && drillWidth > 0) 
                        {
                            double ratio = gerberWidth / drillWidth;
                            double[] knownFactors = { 1.0, 10.0, 100.0, 0.1, 0.01, 25.4, 2.54, 0.03937 };
                            scale = knownFactors.OrderBy(f => Math.Abs(f - ratio)).First();
                            if (ratio > 1.5 || ratio < 0.6) scale = scale; 
                            else scale = 1.0;
                        }

                        var sgmDoc = new SgmDocument { Uom = readResult.Item1.Uom };
                        foreach (var op in readResult.Item1.Operations)
                        {
                            if (op is BoardFlow.Formats.Excellon.Entities.DrillOperation drill) {
                                var diameter = readResult.Item1.ToolsMap.TryGetValue(drill.ToolNumber, out var d) ? (double)d : 0.8;
                                sgmDoc.GraphicElements.Add(new Dot { 
                                    CenterPoint = new BoardFlow.Formats.Sgm.Entities.Point(drill.StartPoint.X * scale, drill.StartPoint.Y * scale), 
                                    Diameter = diameter * scale 
                                });
                            }
                            else if (op is BoardFlow.Formats.Excellon.Entities.MillOperation mill) {
                                var sp = new BoardFlow.Formats.Sgm.Entities.Point(mill.StartPoint.X * scale, mill.StartPoint.Y * scale);
                                var path = new BoardFlow.Formats.Sgm.Entities.GraphicElements.Path { StrokeWidth = 0.5 };
                                foreach(var mp in mill.MillParts) {
                                    var ep = new BoardFlow.Formats.Sgm.Entities.Point(mp.EndPoint.X * scale, mp.EndPoint.Y * scale);
                                    path.Curves.Add(new BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves.Line { PointFrom = sp, PointTo = ep });
                                    sp = ep;
                                }
                                sgmDoc.GraphicElements.Add(path);
                            }
                        }
                        
                        var svgDoc = BoardFlow.Converters.SgmToSvg.SgmToSvgConverter.Convert(sgmDoc);
                        parsedDocuments.Add((drillFile.Name, svgDoc));
                    } else { failedFiles.Add(drillFile.Name + " (Нет операций)"); }
                } catch (Exception ex) { failedFiles.Add(drillFile.Name + $" ({ex.Message})"); }
            }

            foreach (var doc in parsedDocuments)
            {
                var svgFileName = Path.Combine(svgOutputDir, $"{doc.FileName}.svg");
                SvgWriter.Write(doc.SvgDoc, svgFileName, globalGerberBounds);
            }

            if (failedFiles.Count > 0)
            {
                await File.WriteAllLinesAsync(Path.Combine(svgOutputDir, "failed_layers.txt"), failedFiles);
            }
            
            // Сохраняем список проигнорированных файлов
            if (ignoredFiles.Count > 0)
            {
                await File.WriteAllLinesAsync(Path.Combine(svgOutputDir, "ignored_files.txt"), ignoredFiles);
            }

            context.Response.StatusCode = 302;
            context.Response.SetHeader("Location", $"/pcb/viewer/{orderId}");
        }
        finally
        {
            if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);
        }
    }
}