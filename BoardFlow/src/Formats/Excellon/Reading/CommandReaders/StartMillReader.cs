using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public class StartMillReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {
    public ExcellonCommandType[] GetNextLikelyTypes() {
        return [ExcellonCommandType.LinearMillOperation, ExcellonCommandType.ArcMillOperation, ExcellonCommandType.EndMill];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ctx.CurLine == "M15";
    }
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        if (ctx.CurMillOperation != null) {
            ctx.WriteError("Начало фрезерования при незавершенном фрезеровании.");
        }
        ctx.CurMillOperation = new MillOperation() {
            StartPoint = ctx.CurPoint
        };
    }
}