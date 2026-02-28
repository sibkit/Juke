using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public class CommentReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {
    public GerberCommandType[] GetNextLikelyTypes() {
        return [GerberCommandType.Comment];
    }
    public bool Match(GerberReadingContext ctx) {
        return ctx.CurLine.StartsWith("G04");
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        ctx.WriteInfo(ctx.CurLine[3..^1]);
    }
}