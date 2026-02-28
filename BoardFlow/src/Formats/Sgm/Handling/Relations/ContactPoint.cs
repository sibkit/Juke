using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;

namespace BoardFlow.Formats.Sgm.Handling.Relations;

public class ContactPoint {
    public required Point Point { get; init; }
    public required double T { get; init; }
    public required double BaseT { get; init; }
    public required ICurve Curve { get; init; }
    public required ICurve BaseCurve { get; init; }
}