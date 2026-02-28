using BoardFlow.Formats.Common;
using BoardFlow.Formats.Common.Entities;
using BoardFlow.Formats.Common.Reading;
using BoardFlow.Formats.Excellon.Entities;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Excellon.Reading;

public enum CoordinatesMode {
    Incremental,
    Absolute
}

public struct CoordinatesDefineState {
    public bool CalculateValueDetected = false;
    public bool AccurateValueDetected = false;
    public bool DifferentLineLengthsDetected = false;
    public bool UndefinedScaleDetected = false;
    public CoordinatesDefineState() { }
}

public class ExcellonReadingContext: ReadingContext {

    public bool UndefinedFormatDetected { get; set; } = false;
    public NumberFormat NumberFormat { get; set; } = new(null, null);
    
    public int? CurToolNumber { get; set; }
    public CoordinatesMode CoordinatesMode { get; set; } = CoordinatesMode.Absolute;
    public Pattern? CurPattern { get; set; }
    public Point CurPoint { get; set; }

    public MillOperation? CurMillOperation { get; set; } = null;
    public Uom? Uom {get; set;} = null;
    
    public CoordinatesDefineState CoordinatesDefineState = new();
}