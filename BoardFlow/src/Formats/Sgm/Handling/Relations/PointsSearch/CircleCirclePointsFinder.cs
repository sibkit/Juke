using System;
using System.Collections.Generic;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Formats.Sgm.Handling.Relations.PointsSearch;

public class CircleCirclePointsFinder: IPointsFinder<Arc, Arc> {
    
    public (List<Point> points, bool isMatch) FindContactPoints(Arc arc1, Arc arc2) {
        
        var p1C = Geometry.ArcCenter(arc1);
        var p2C = Geometry.ArcCenter(arc2);

        if (p1C == p2C && Math.Abs(arc1.Radius - arc2.Radius) < Geometry.Accuracy) {
            return ([], true);
        }
        
        var p1R = arc1.Radius;
        var p2R = arc2.Radius;

        var isMatch = Math.Abs(p1R - p2R) < Geometry.Accuracy && p1C == p2C;
        
        var d = Math.Sqrt((p2C.X-p1C.X)*(p2C.X-p1C.X)+(p2C.Y-p1C.Y)*(p2C.Y-p1C.Y));
        if(d>p1R+p2R || d<Math.Abs(p1R-p2R) || d==0)
            return ([], isMatch);
        
        var a = (p1R*p1R - p2R*p2R + d*d)/(2d*d);
        var h = Math.Sqrt(p1R*p1R - a*a);

        var xm = p1C.X + (a / d) * (p2C.X - p1C.X);
        var ym = p1C.Y + (a / d) * (p2C.Y - p1C.Y);
        
        var x1 = xm+(h/d)*(p2C.Y - p1C.Y);
        var y1 = ym-(h/d)*(p2C.X - p1C.X);

        var x2 = xm-(h/d)*(p2C.Y - p1C.Y);
        var y2 = ym+(h/d)*(p2C.X - p1C.X);
        
        if (double.IsNaN(x1) || double.IsNaN(y1)) {
            return ([], false);
        }

        var p1 = new Point(x1, y1);
        var p2 = new Point(x2, y2);
        
        if(p1==p2)
            return ([p1], isMatch);
        
        return ([p1, p2], isMatch);
    }
}