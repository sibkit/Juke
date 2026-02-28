namespace BoardFlow.Formats.Sgm.Entities.GraphicElements;

public class Dot: IGraphicElement {
    public Point CenterPoint{get; set; }
    public double Diameter{get; init; }
    public Bounds Bounds => new(CenterPoint.X-Diameter/2, CenterPoint.Y-Diameter/2, CenterPoint.X+Diameter/2, CenterPoint.Y+Diameter/2);
    
    public void Move(double dx, double dy) {
        CenterPoint = CenterPoint with {
            X=CenterPoint.X+dx,
            Y=CenterPoint.Y+dy
        };
    }
}