using System.Collections.Generic;
using System.IO;
using System.Linq;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;
using BoardFlow.Formats.Excellon.Reading.CommandReaders;
using ArcMillOperationReader = BoardFlow.Formats.Excellon.Reading.CommandReaders.ArcMillOperationReader;
using SetToolReader = BoardFlow.Formats.Excellon.Reading.CommandReaders.SetToolReader;
using SetUomFormatReader = BoardFlow.Formats.Excellon.Reading.CommandReaders.SetUomFormatReader;
using ToolDefineReader = BoardFlow.Formats.Excellon.Reading.CommandReaders.ToolDefineReader;

namespace BoardFlow.Formats.Excellon.Reading;

using ArcMillOperationReader = ArcMillOperationReader;
using SetToolReader = SetToolReader;
using SetUomFormatReader = SetUomFormatReader;
using ToolDefineReader = ToolDefineReader;

public class ExcellonReader: CommandsFileReader<ExcellonCommandType,ExcellonReadingContext, Entities.ExcellonDocument> {
    
    public static readonly ExcellonReader Instance = new();

    private static Dictionary<ExcellonCommandType, ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument>> GetHandlers() {
        var handlers = new Dictionary<ExcellonCommandType, ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument>> {
            { ExcellonCommandType.StartHeader, new StartHeaderReader() },
            { ExcellonCommandType.Comment, new CommentReader() },
            { ExcellonCommandType.EndHeader, new EndHeaderReader() },
            { ExcellonCommandType.SetUomFormat, new SetUomFormatReader() },
            { ExcellonCommandType.SetTool, new SetToolReader() },
            { ExcellonCommandType.DrillOperation, new DrillingOperationReader() },
            { ExcellonCommandType.ToolDefine, new ToolDefineReader() },
            { ExcellonCommandType.Ignored, new IgnoredFormatReader() },
            { ExcellonCommandType.BeginPattern, new BeginPatternReader() },
            { ExcellonCommandType.EndPattern, new EndPatternReader() },
            { ExcellonCommandType.RepeatPattern, new RepeatPatternReader() },
            { ExcellonCommandType.SetDrillMode, new SetDrillModeReader() },
            { ExcellonCommandType.EndProgram, new EndProgramCommandReader() },
            { ExcellonCommandType.SetCoordinatesMode, new SetCoordinatesModeReader() },
            { ExcellonCommandType.RoutOperation, new RoutOperationReader() },
            { ExcellonCommandType.StartMill, new StartMillReader() },
            { ExcellonCommandType.EndMill, new EndMillReader() },
            { ExcellonCommandType.LinearMillOperation, new LinearMillOperationReader() },
            { ExcellonCommandType.ArcMillOperation, new ArcMillOperationReader() }
        };
        return handlers;
    }
    
    private ExcellonReader():base(GetHandlers(),[ExcellonCommandType.StartHeader]){ }
    protected override IEnumerable<string> ExcludeCommands(TextReader reader) {
        return reader.ReadToEnd().Split('\n','\r').Where(str => str!="");
    }
}