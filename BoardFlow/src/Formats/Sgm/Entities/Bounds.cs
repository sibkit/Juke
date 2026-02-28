namespace BoardFlow.Formats.Sgm.Entities;

public class Bounds {
    
    public double MinX { get; }
    public double MinY { get; }
    public double MaxX { get; }
    public double MaxY { get; }

    public Point MinPoint => new(MinX, MinY);
    public Point MaxPoint => new(MaxX, MaxY);
    
    public Bounds(Point minPoint, Point maxPoint) : this(minPoint.X, minPoint.Y, maxPoint.X, maxPoint.Y) { }

    public Bounds(double minX, double minY, double maxX, double maxY) {
        // if(minX > maxX || minY > maxY)
        //     throw new Exception("Bounds: minPoint must be greater than or equal to maxPoint");
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }
    
    public double GetWidth() {
        return MaxX - MinX;
    }

    public double GetHeight() {
        return MaxY - MinY;
    }

    public bool Contains(Point p) {
        return p.X >= MinX && p.X <= MaxX && p.Y >= MinY && p.Y <= MaxY;
    }

    private static bool IsIntersected(double min1, double min2, double max1, double max2) {
        return max1 >= min2 && min1 <= max2;
    }
    
    public bool IsIntersected(Bounds b) {
        
        return (MaxX>=b.MinX && MinX<=b.MaxX) && (MaxY>=b.MinY && MinY<=b.MaxY);
    }

    public Bounds ExtendBounds(Bounds b) {
        if (b == null)
            return Clone();
        return new Bounds(
            MinX < b.MinX ? MinX : b.MinX,
            MinY < b.MinY ? MinY : b.MinY,
            MaxX > b.MaxX ? MaxX : b.MaxX,
            MaxY > b.MaxY ? MaxY : b.MaxY
        );
    }
    
    public static Bounds Empty() {
        return new Bounds(
            double.PositiveInfinity, 
            double.PositiveInfinity,
            double.NegativeInfinity,
            double.NegativeInfinity
        );
    }

    public Bounds Clone() {
        return new Bounds(MinX, MinY, MaxX, MaxY);
    }
}