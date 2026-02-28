using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public class EndRegionCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return ctx.CurLine == "G37*";
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        if (ctx.CurPathPaintOperation != null) {
            ctx.CurPathPaintOperation.IsClosed = true;
            document.Operations.Add(ctx.CurPathPaintOperation);
            ctx.CurPathPaintOperation = null;
        } else {
            ctx.WriteError("G37 Нет операций");
        }
    }
}