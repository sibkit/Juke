using System;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Formats.Sgm.Handling.Relations.PointsSearch;

public static class TCalculator {
    
    public static double CalculateT(ICurve curve, Point ip) {
        return curve switch {
            Line line => CalculateT(line, ip),
            Arc arc => CalculateT(arc, ip),
            _ => throw new ArgumentOutOfRangeException(nameof(curve), curve, null)
        };
    }

    public static Point CalculatePoint(ICurve curve, double t) {
        return curve switch {
            Line linePart => CalculatePoint(linePart, t),
            Arc arcPart => CalculatePoint(arcPart, t),
            _ => throw new ArgumentOutOfRangeException(nameof(curve), curve, null)
        };
    }

    public static Point CalculatePoint(Line line, double t) {
        return new Point(
            line.PointFrom.X + t * (line.PointTo.X - line.PointFrom.X),
            line.PointFrom.Y + t * (line.PointTo.Y - line.PointFrom.Y)
        );
    }

    public static Point CalculatePoint(Arc arc, double t) {
        return new Point(
            arc.PointFrom.X + t * (arc.PointTo.X - arc.PointFrom.X),
            arc.PointFrom.Y + t * (arc.PointTo.Y - arc.PointFrom.Y)
        );
    }
    
    public static double CalculateT(Line line, Point ip) {
        var dx = Math.Abs(line.PointTo.X - line.PointFrom.X);
        var dy = Math.Abs(line.PointTo.Y - line.PointFrom.Y);
        if (dx > dy) {
            return (ip.X - line.PointFrom.X) / (line.PointTo.X - line.PointFrom.X);
        } else {
            return (ip.Y - line.PointFrom.Y) / (line.PointTo.Y - line.PointFrom.Y);
        }
    }
    
    public static double CalculateT(Arc arc, Point ip) {
        //sp - startPoint
        //ep - endPoint
        //ip - intersectionPoint
        
        //PrX - X axe projection
        //PrY - Y axe projection

        var cp = Geometry.ArcCenter(arc);
        
        var spPrX = arc.PointFrom.X-cp.X;
        var spPrY = arc.PointFrom.Y-cp.Y;
        
        var epPrX = arc.PointTo.X-cp.X;
        var epPrY = arc.PointTo.Y-cp.Y;
        
        var ipPrX = ip.X-cp.X;
        var ipPrY = ip.Y-cp.Y;
        
        var startAngle = Angles.PositiveNormalize(Math.Atan2(spPrY, spPrX));
        var endAngle = Angles.PositiveNormalize(Math.Atan2(epPrY, epPrX));
        var intersectionAngle = Angles.PositiveNormalize(Math.Atan2(ipPrY, ipPrX));
        
        double fullArcAngle;
        switch (arc.RotationDirection) {
            case RotationDirection.Clockwise:
                //знак с минусом
                fullArcAngle = Angles.NegativeNormalize(endAngle - startAngle);
                return Angles.NegativeNormalize(intersectionAngle-startAngle) / fullArcAngle;
            case RotationDirection.CounterClockwise:
                //знак с плюсом
                fullArcAngle = Angles.PositiveNormalize(endAngle - startAngle);
                return Angles.PositiveNormalize(intersectionAngle-startAngle) / fullArcAngle;
            default:
                throw new Exception("TCalculator : CalculateT");
        }
    }
}