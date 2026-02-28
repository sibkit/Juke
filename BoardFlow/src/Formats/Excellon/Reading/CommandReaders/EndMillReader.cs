using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public class EndMillReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {
    public ExcellonCommandType[] GetNextLikelyTypes() {
        return [ExcellonCommandType.RoutOperation, ExcellonCommandType.DrillOperation, ExcellonCommandType.SetTool];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ctx.CurLine is "M16" or "M17";
    }
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        if (ctx.CurLine == "M17" && ctx.Lines[ctx.CurIndex-1] == "M16") {
            return;
        }
        if (ctx.CurMillOperation == null) {
            ctx.WriteError("Завершение не начатой операции фрезерования.");
            ctx.CurMillOperation = null;
            return;
        }
        document.Operations.Add(ctx.CurMillOperation);
        ctx.CurMillOperation = null;
    }
}