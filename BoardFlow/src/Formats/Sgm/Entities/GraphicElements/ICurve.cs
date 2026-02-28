namespace BoardFlow.Formats.Sgm.Entities.GraphicElements;

public interface ICurve {
    
    CurvesOwner Owner { get; set; }
    
    Point PointFrom { get; }
    Point PointTo { get; }
    
    Bounds Bounds { get; }

    ICurve GetReversed();
    void Reverse();
    void Move(double dx, double dy);

}