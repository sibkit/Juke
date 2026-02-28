using BoardFlow.Formats.Sgm.Entities.GraphicElements;

namespace BoardFlow.Formats.Sgm.Handling;



public class TransitionPoint {
    public ICurve InCurve { get; init; }
    public ICurve OutCurve { get; init; }
    public Contour Contour { get; init; }
}

