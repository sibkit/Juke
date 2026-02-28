using System;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;
using ApplicationException = System.ApplicationException;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public partial class RepeatPatternReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {
    
    public ExcellonCommandType[] GetNextLikelyTypes() {
        return [ExcellonCommandType.RepeatPattern];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ctx.CurLine.StartsWith("M02");
    }
    
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        if (ctx.CurLine == "M02") {
            if (ctx.CurPattern == null || ctx.CurPattern.State == PatternState.Opened)
                throw new ApplicationException("Pattern is null or opened (1)");
            ctx.CurPattern = null;
        } else {
            var sc = ctx.CurLine.Split("M02", StringSplitOptions.RemoveEmptyEntries)[0];
            var readedPoint = ExcellonCoordinates.ReadCoordinate(sc, ctx);
            if (readedPoint == null) throw new ApplicationException("Readed point is null");
            if (ctx.CurPattern == null || ctx.CurPattern.State == PatternState.Opened)
                throw new ApplicationException("Pattern is null or opened (2)");

            var pattern = ctx.CurPattern!;
            foreach (var operation in pattern.MachiningOperations) {
                document.Operations.Add(operation.CloneWithShift(readedPoint.Value));
            }
        }
    }
}