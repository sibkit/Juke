using System;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;

namespace BoardFlow.Formats.Excellon.Reading.CommandReaders;

public class SetCoordinatesModeReader: ICommandReader<ExcellonCommandType, ExcellonReadingContext, Entities.ExcellonDocument> {
    public ExcellonCommandType[] GetNextLikelyTypes() {
        return [ExcellonCommandType.DrillOperation];
    }
    public bool Match(ExcellonReadingContext ctx) {
        return ctx.CurLine is "G90" or "G91" or "ICI,OFF" or "ICI,ON" or "ICI";
    }
    public void WriteToProgram(ExcellonReadingContext ctx, Entities.ExcellonDocument document) {
        ctx.CoordinatesMode = ctx.CurLine switch {
            "G90" => CoordinatesMode.Absolute,
            "G91" => CoordinatesMode.Incremental,
            "ICI" => CoordinatesMode.Incremental,
            "ICI,ON" => CoordinatesMode.Incremental,
            "ICI,OFF" => CoordinatesMode.Absolute,
            _ => throw new Exception("Unknown CoordinatesMode: " + ctx.CurLine)
        };
    }
}