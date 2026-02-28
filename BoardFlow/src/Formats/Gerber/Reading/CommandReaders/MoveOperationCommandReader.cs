using System.Text.RegularExpressions;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public partial class MoveOperationCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {
    
    [GeneratedRegex("^(?:(X)([+-]?[0-9.]+))?(?:(Y)([+-]?[0-9.]+))?D02\\*$")]
    private static partial Regex MatchRegex();
    
    
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return MatchRegex().IsMatch(ctx.CurLine);
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        var m = MatchRegex().Match(ctx.CurLine);
        var xs = m.Groups[2].Value;
        var ys = m.Groups[4].Value;
        
        if (ctx.NumberFormat == null) {
            ctx.WriteError("Не определен формат чисел.");
            ctx.ContinueHandle = false;
            return;
        }
        
        ctx.CurCoordinate = Coordinates.ParseCoordinate(ctx.NumberFormat!,xs,ys);
    }
}