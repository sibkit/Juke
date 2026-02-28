using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Gerber.Entities;

public class ArcPathPart: IPathPart {
    public Point EndPoint { get; set; }
    public double IOffset { get; set; }
    public double JOffset { get; set; }
    public RotationDirection RotationDirection { get; set; }
    public QuadrantMode QuadrantMode { get; set; } 
}