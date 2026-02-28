using System;
using BoardFlow.Formats.Common;
using BoardFlow.Formats.Common.Entities;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public class SetCoordinateModeCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return ctx.CurLine is "G90*" or "G91*";
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        ctx.CoordinatesMode = ctx.CurLine switch {
            "G90*" => CoordinatesMode.Absolute,
            "G91*" => CoordinatesMode.Incremental,
            _ => throw new Exception("Unknown coordinates mode")
        };
    }
}