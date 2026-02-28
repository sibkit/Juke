using BoardFlow.Formats.Common;
using BoardFlow.Formats.Common.Entities;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Gerber.Entities;
using Exception = System.Exception;

namespace BoardFlow.Formats.Gerber.Reading.CommandReaders;

public class SetUomFormatCommandReader: ICommandReader<GerberCommandType, GerberReadingContext, GerberDocument> {
    public GerberCommandType[] GetNextLikelyTypes() {
        return [];
    }
    public bool Match(GerberReadingContext ctx) {
        return ctx.CurLine is "MOIN*" or "MOMM*";
    }
    public void WriteToProgram(GerberReadingContext ctx, GerberDocument document) {
        document.Uom = ctx.CurLine switch {
            "MOIN*" => Uom.Inch,
            "MOMM*" => Uom.Metric,
            _ => throw new Exception("Unknown UOM")
        };
    }
}