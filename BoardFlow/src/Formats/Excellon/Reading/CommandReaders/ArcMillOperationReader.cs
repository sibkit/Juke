using System.Text.RegularExpressions;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public partial class ArcMillOperationReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {
    
    private static readonly Regex ReArcMill = ArcMillRegex();
    public ExcellonCommandType[] GetNextLikelyTypes() {
        return [
            ExcellonCommandType.ArcMillOperation, 
            ExcellonCommandType.LinearMillOperation, 
            ExcellonCommandType.EndMill
        ];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ctx.CurLine.StartsWith("G02") || ctx.CurLine.StartsWith("G03");
    }
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        var match = ReArcMill.Match(ctx.CurLine);
        if (match.Success) {
            
            var gCode = match.Groups[1].Value;
            var coordinate = match.Groups[2].Value;
            var a = match.Groups[4].Value;
            
            //ctx.
            
        } else {
            ctx.WriteError("Не удалось распознать строку: \"" + ctx.CurLine + "\"");
        }
    }

    
    [GeneratedRegex("^(G02|G03)((?:[XY][+-]?[0-9.]+)?(?:[XY][+-]?[0-9.]+))?(?:(A)([+-]?[0-9.]+))$")]
    private static partial Regex ArcMillRegex();
}