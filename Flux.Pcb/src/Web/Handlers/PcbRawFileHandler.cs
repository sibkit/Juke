/* PcbRawFileHandler.cs */
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;

namespace Flux.Pcb.Web.Handlers;

public class PcbRawFileHandler : IRequestHandler
{
    public async Task HandleAsync(IHttpContext context)
    {
        var orderId = context.Request.RouteValues["orderId"]?.ToString();
        var fileName = context.Request.RouteValues["fileName"]?.ToString();

        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(fileName))
        {
            context.Response.StatusCode = 404;
            return;
        }

        fileName = Path.GetFileName(fileName); // Защита пути
        var zipPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Orders", orderId, "archive.zip");

        if (!File.Exists(zipPath))
        {
            context.Response.StatusCode = 404;
            return;
        }

        // Подготавливаем уникальную временную директорию для распаковки
        var tempDir = Path.Combine(Path.GetTempPath(), "flux_pcb_raw_downloads", Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var tempFilePath = Path.Combine(tempDir, fileName);

        try
        {
            // Извлекаем нужный файл из ZIP-архива
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                var entry = archive.Entries.FirstOrDefault(e => e.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (entry == null)
                {
                    context.Response.StatusCode = 404;
                    return;
                }
                entry.ExtractToFile(tempFilePath, overwrite: true);
            }

            // Определяем Content-Type
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = ext switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".pdf" => "application/pdf",
                ".txt" or ".csv" => "text/plain",
                ".xml" => "application/xml",
                _ => "application/octet-stream"
            };
            
            context.Response.SetContentType(contentType);
            
            // Если браузер не умеет открывать файл, предлагаем его скачать
            if (contentType == "application/octet-stream")
            {
                context.Response.SetHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            }

            // Отдаем файл клиенту потоком, дожидаясь завершения
            await using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await fs.CopyToAsync(context.Response.Body);
            }
        }
        finally
        {
            // Очищаем: удаляем временную директорию и сам извлеченный файл
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}