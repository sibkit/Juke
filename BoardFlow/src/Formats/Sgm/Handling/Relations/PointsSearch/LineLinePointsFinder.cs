using System;
using System.Collections.Generic;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Formats.Sgm.Handling.Relations.PointsSearch;

public class LineLinePointsFinder: IPointsFinder<Line, Line> {

    private bool Eq(double v1, double v2) {
        return Math.Abs(v1 - v2) < Geometry.Accuracy;
    }
    
    public (List<Point> points, bool isMatch) FindContactPoints(Line line1, Line line2) {
        var c1X1 = line1.PointFrom.X;
        var c1Y1 = line1.PointFrom.Y;
        var c1X2 = line1.PointTo.X;
        var c1Y2 = line1.PointTo.Y;
        var c2X1 = line2.PointFrom.X;
        var c2Y1 = line2.PointFrom.Y;
        var c2X2 = line2.PointTo.X;
        var c2Y2 = line2.PointTo.Y;
        
        var c1Dx = c1X2 - c1X1;
        var c1Dy = c1Y2 - c1Y1;
        var c2Dx = c2X2 - c2X1;
        var c2Dy = c2Y2 - c2Y1;

        if (Eq(c1Dx, 0) && Eq(c2Dx, 0)) {
            if (Eq(c1X1, c2X1))
                return ([], true);
            return ([], false);
        }
        
        if (Eq(c1Dy, 0) && Eq(c2Dy, 0)) {
            if (Eq(c1Y1, c2Y1))
                return ([], true);
            return ([], false);
        }


        
        var k1 = c1Dy / c1Dx;
        var k2 = c2Dy / c2Dx;

        var b1 = c1Y1 - c1X1 * k1;
        var b2 = c2Y1 - c2X1 * k2;

        
        if (Eq(c1Dx, 0)) {
            return ([new Point(c1X1,k2*c1X1+b2)], false);
        }
        
        if (Eq(c2Dx, 0)) {
            return ([new Point(c2X1,k1*c2X1+b1)], false);
        }
        
        var x = (b1 - b2) / (k2 - k1);
        var y = k1 * x + b1;
        //var x = ((c1X1 * c1Y2 - c1Y1 * c1X2) * c2Dx - (c2X1 * c2Y2 - c2Y1 * c2X2) * c1Dx) / (c1Dx * c2Dy - c1Dy * c2Dx);
        //double y;
        // y = ((x - c2X1) * c2Dy / c2Dx) + c2Y1;


        if (!double.IsFinite(x)) {

            //var k1 = (c1Y1 - c1Y2) / (c1X1 - c1X2);
            //var k2 = (c2Y1 - c2Y2) / (c2X1 - c2X2);

            if (double.IsInfinity(k1)) {
                if (double.IsInfinity(k2)) {
                    if (Math.Abs(c1X1 - c2X1) < Geometry.Accuracy) {
                        return ([], true);
                    } 
                    return ([], false);
                    
                }
            } 
            
            if (Math.Abs(k1 - k2) < Geometry.Accuracy) {
                //var b1 = c1Y2 - k1 * c1X2;
                //var b2 = c2Y2 - k2 * c2X2;
                if (Math.Abs(b1 - b2) < Geometry.Accuracy) {
                    return ([], true);
                }
            }
            return ([], false);
        }
        
        return([new Point(x, y)], false);
    }
}