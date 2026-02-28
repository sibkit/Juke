using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public class LinearMillOperationReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {
    public ExcellonCommandType[] GetNextLikelyTypes() {
        return [ExcellonCommandType.LinearMillOperation, ExcellonCommandType.ArcMillOperation];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ctx.CurLine.StartsWith("G01");
    }
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        var sc = ctx.CurLine[3..];
        var coordinate = ExcellonCoordinates.ReadCoordinate(sc, ctx);
        if (coordinate == null) {
            ctx.WriteError("Invalid coordinate: "+ctx.CurLine);
        } else {
            if (ctx.CurMillOperation == null) {
                ctx.WriteError("Операция фрезерования при поднятом шпинделе");
            } else {
                var part = new LinearMillPart(coordinate.Value);
                ctx.CurMillOperation.MillParts.Add(part);
            }
            ctx.CurPoint = coordinate.Value;
        }
    }
}