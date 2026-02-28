using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public class SetQuadrantModeCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return ctx.CurLine is "G74*" or "G75*";
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        ctx.QuadrantMode = ctx.CurLine == "G74*" ? QuadrantMode.Single : QuadrantMode.Multi;
    }
}