using System;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Formats.Sgm.Handling;

public static class CloneExtension {
    public static Shape Clone(this Shape shape) {
        var result = new Shape();
        foreach(var oc in shape.OuterContours)
            result.OuterContours.Add(oc.Clone());
        foreach(var ic in shape.InnerContours)
            result.InnerContours.Add(ic.Clone());
        return result;
    }

    public static Contour Clone(this Contour contour) {
        var result = new Contour();
        foreach(var curve in contour.Curves)
            result.Curves.Add(curve.Clone());
        return result;
    }

    public static ICurve Clone(this ICurve curve) {
        return curve switch {
            Line line => line.Clone(),
            Arc arc => arc.Clone(),
            _ => throw new Exception("CloneExtension: Clone(ICurve)")
        };
    }
    
    public static Line Clone(this Line line) {
        return new Line{
            PointTo = line.PointTo,
            PointFrom = line.PointFrom
        };

    }
    
    public static Arc Clone(this Arc arc) {
        return new Arc {
            PointTo = arc.PointTo,
            PointFrom = arc.PointFrom,
            Radius = arc.Radius,
            IsLargeArc = arc.IsLargeArc,
            RotationDirection = arc.RotationDirection
        };
    }
}