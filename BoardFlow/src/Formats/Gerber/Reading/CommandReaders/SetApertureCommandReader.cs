using System.Text.RegularExpressions;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public partial class SetApertureCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {

    [GeneratedRegex("^(?:G54)*D([0-9]+)\\*$")]
    private static partial Regex MyRegex();
    
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return MyRegex().IsMatch(ctx.CurLine);
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        var m = MyRegex().Match(ctx.CurLine);
        var apCode = int.Parse(m.Groups[1].Value);
        if (document.Apertures.ContainsKey(apCode)) {
            ctx.CurApertureCode = apCode;
        } else {
            ctx.WriteError("Не найдена аппертура с кодом: "+apCode);
        }
    }
}