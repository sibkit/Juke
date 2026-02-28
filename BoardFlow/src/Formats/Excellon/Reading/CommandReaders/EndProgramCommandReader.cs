using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public class EndProgramCommandReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {
    public ExcellonCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ctx.CurLine == "M30";
    }
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        //Do nothing
    }
}