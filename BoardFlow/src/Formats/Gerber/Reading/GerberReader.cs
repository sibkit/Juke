using System.Collections.Generic;
using System.IO;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;
using BoardFlow.Formats.Gerber.Reading.CommandReaders;

namespace BoardFlow.Formats.Gerber.Reading;

public class GerberReader: CommandsFileReader<GerberCommandType, GerberReadingContext, GerberDocument> {

    public static readonly GerberReader Instance = new();
    
    private static Dictionary<GerberCommandType, ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument>> GetHandlers() {
        var handlers = new Dictionary<GerberCommandType, ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument>> {
            { GerberCommandType.LineSegmentOperation, new LineSegmentOperationReader() },
            { GerberCommandType.Comment, new CommentReader() },
            { GerberCommandType.FormatSpecification, new FormatSpecificationCommandReader() },
            { GerberCommandType.MoveOperation, new MoveOperationCommandReader() },
            { GerberCommandType.SetLcMode, new SetLcModeCommandReader() },
            { GerberCommandType.SetCoordinates, new SetCoordinateModeCommandReader() },
            { GerberCommandType.Ignored, new IgnoredCommandReader() },
            { GerberCommandType.SetUom, new SetUomFormatCommandReader() },
            { GerberCommandType.DefineAperture, new DefineApertureCommandReader() },
            { GerberCommandType.SetAperture , new SetApertureCommandReader() },
            { GerberCommandType.ArcSegmentOperation , new ArcSegmentOperationCommandReader() },
            { GerberCommandType.DefineApertureMacro, new DefineApertureTemplateCommandReader() },
            { GerberCommandType.FlashOperation, new FlashOperationCommandReader() },
            { GerberCommandType.BeginRegion , new BeginRegionCommandReader() },
            { GerberCommandType.EndRegion , new EndRegionCommandReader() },
            { GerberCommandType.SetQuadrantMode, new SetQuadrantModeCommandReader() },
        };
        return handlers;
    }
    
    private GerberReader():base(GetHandlers(),[]){ }

    private bool CheckForExCommandStart(string line, int index) {
        return line.Length>index+3 && line[(index+1)..(index+3)] == "AM";
    }
    
    protected override IEnumerable<string> ExcludeCommands(TextReader reader) {
        var exOpened = false;
        string? curCommand = null;
        
        while (reader.ReadLine() is { } line) {
            for (var i = 0; i < line.Length; i++) {
                switch (line[i]) {
                    case '*':
                        if (!exOpened) {
                            if (curCommand != null) {
                                yield return curCommand + line[i];
                                curCommand = null;
                            }
                        } else {
                            curCommand += line[i];
                        }

                        break;
                    case '%':
                        if (exOpened) {
                            exOpened = false;
                            yield return curCommand + '%';
                            curCommand = null;
                        }

                        if (CheckForExCommandStart(line, i)) {
                            exOpened = true;
                            curCommand = "" + line[i];
                        }

                        break;
                    default:
                        curCommand += line[i];
                        break;
                }
            }
        }
    }
}