using System;
using System.Collections.Generic;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Formats.Sgm.Handling.Relations.PointsSearch;

public class LineCirclePointsFinder : IPointsFinder<Line, Arc> {
    
    public (List<Point> points, bool isMatch) FindContactPoints(Line line, Arc arc) {
        
        var p2C = Geometry.ArcCenter(arc);
        
        var p1X1 = line.PointFrom.X - p2C.X;
        var p1Y1 = line.PointFrom.Y - p2C.Y;
        
        var p1X2 = line.PointTo.X - p2C.X;
        var p1Y2 = line.PointTo.Y - p2C.Y;

        var p1Dx = p1X2 - p1X1;
        var p1Dy = p1Y2 - p1Y1;
        
        var a = p1Dy;
        var b = -p1Dx; 
        var c = p1Dx * p1Y1 - p1Dy * p1X1;

        var d = Math.Abs(c)/Math.Sqrt(a*a + b*b);
        if (d > arc.Radius)
            return ([], false);

        
        var zpX = -(a * c) / (a * a + b * b);
        var zpY = -(b * c) / (a * a + b * b);

        if (Math.Abs(d - arc.Radius) < Geometry.Accuracy) {
            return ([new Point(zpX+p2C.X, zpY+p2C.Y)], false);
        }
        
        var k = Math.Sqrt(arc.Radius*arc.Radius - d*d);

        var mult = Math.Sqrt(k * k / (a * a + b * b));
        
        var pI1 = new Point(zpX + b * mult, zpY - a * mult);
        var pI2 = new Point(zpX - b * mult, zpY + a * mult);

        return ([
            new Point(pI1.X + p2C.X, pI1.Y + p2C.Y),
            new Point(pI2.X + p2C.X, pI2.Y + p2C.Y)
        ], false);
    }
}