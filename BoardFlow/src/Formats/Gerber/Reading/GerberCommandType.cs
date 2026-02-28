namespace BoardFlow.Formats.Gerber.Reading;

public enum GerberCommandType {
    Comment,
    FormatSpecification,
    SetUom,
    DefineAperture,
    SetCoordinates,
    SetAperture,
    DefineApertureMacro,
    FlashOperation,
    MoveOperation,
    ArcSegmentOperation,
    LineSegmentOperation,
    SetLcMode,
    SetQuadrantMode,
    Ignored,
    BeginRegion,
    EndRegion,
}