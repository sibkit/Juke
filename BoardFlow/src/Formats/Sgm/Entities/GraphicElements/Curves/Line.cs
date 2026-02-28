namespace BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

public class Line: ICurve {
    
    public CurvesOwner Owner { get; set; }
    public required Point PointTo { get; set; }
    public required Point PointFrom { get; set; }

    
    public Bounds Bounds {
        get {
                double minX;
                double minY;
                double maxX;
                double maxY;

                if (PointFrom.X < PointTo.X) {
                    minX = PointFrom.X;
                    maxX = PointTo.X;
                } else {
                    minX = PointTo.X;
                    maxX = PointFrom.X;
                }

                if (PointFrom.Y < PointTo.Y) {
                    minY = PointFrom.Y;
                    maxY = PointTo.Y;
                } else {
                    minY = PointTo.Y;
                    maxY = PointFrom.Y;
                }

                return new Bounds(minX, minY, maxX, maxY);
        }
    }

    public ICurve GetReversed() {
        var result = new Line {
            PointFrom = PointTo,
            PointTo = PointFrom
        };
        return result;
    }

    public void Reverse() {
        (PointFrom, PointTo) = (PointTo, PointFrom);
    }
    
    public void Move(double dx, double dy) {
        PointTo = PointTo with {
            X = PointTo.X + dx,
            Y = PointTo.Y + dy
        };
        PointFrom = PointFrom with {
            X = PointFrom.X + dx,
            Y = PointFrom.Y + dy
        };
    }
}