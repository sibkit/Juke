using System;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Formats.Sgm.Handling;

public readonly struct ArcWay(RotationDirection direction, bool isLarge) {
    public RotationDirection RotationDirection { get; init; } = direction;
    public bool IsLarge { get; init; } = isLarge;
}

public static class Geometry {

    public static double Accuracy { get; } = 0.000_000_000_01;

    public static double LineLength(Point sp, Point ep) {
        return Math.Sqrt(Math.Pow(ep.X - sp.X, 2) + Math.Pow(ep.Y - sp.Y, 2));
    }

    public static Point RoundPoint(Point point) {
        return new Point(Math.Round(point.X, 10), Math.Round(point.Y, 10));
    }
    
    public static Point ArcCenter(Arc pp) {
        //находим центр окружности через точки пересечения окружностей с центрами в sp и ep.
        //rd 
        var sp = pp.PointFrom;
        var ep = pp.PointTo;
        
        
        var p0 = new Point((ep.X + sp.X) / 2, (ep.Y + sp.Y) / 2);
        var d = LineLength(sp, ep);
        var a = d / 2;
        var h = Math.Sqrt(pp.Radius*pp.Radius - a*a);
        
        // var x1 = p0.X+(h/d)*(ep.Y-sp.Y);
        // var y1 = p0.Y-(h/d)*(ep.X-sp.X);    
        //
        // var x2 = p0.X - (h/d)*(ep.Y-sp.Y);
        
        var p1 = new Point(
            p0.X + (ep.Y - sp.Y) * h / d, 
            p0.Y - (ep.X - sp.X) * h / d
            );
        var p2 = new Point(
            p0.X - (ep.Y - sp.Y) * h / d, 
            p0.Y + (ep.X - sp.X) * h / d
            );
        return pp.RotationDirection == RotationDirection.Clockwise ? (pp.IsLargeArc ? p2 : p1) : (pp.IsLargeArc ? p1 : p2);
    }

    // public static Point ArcMiddlePoint(Arc arc) {
    //     var cp = ArcCenter(arc);
    //     var theta = Math.Atan2(arc.PointFrom.Y - cp.Y, arc.PointFrom.X - cp.X);
    //     
    //     var beta = Angles.CalculateAngle(arc.PointFrom, arc.PointTo, cp);
    //     beta = arc.RotationDirection switch {
    //         RotationDirection.Clockwise => Angles.NegativeNormalize(beta),
    //         RotationDirection.CounterClockwise => Angles.PositiveNormalize(beta),
    //         _ => beta
    //     };
    //
    //     var mb = beta / 2;
    //
    //     return new Point(
    //         cp.X + arc.Radius * Math.Cos(theta + mb),
    //         cp.Y + arc.Radius * Math.Sin(theta + mb)
    //     );
    //
    //
    // }

    public static ArcWay ArcWay(Point sp, Point ep, Point cp) {
        var angle = Angles.CalculateAngle(sp, ep, cp);

        var vecA = new Vector(ep - sp);
        var vecB = new Vector(cp - sp);
        var crossProduct = Vectors.CrossProduct(vecA, vecB);
        
        return crossProduct > 0 ? 
            new ArcWay(RotationDirection.CounterClockwise, angle > Math.PI) : 
            new ArcWay(RotationDirection.Clockwise, angle > Math.PI);
    }
}

