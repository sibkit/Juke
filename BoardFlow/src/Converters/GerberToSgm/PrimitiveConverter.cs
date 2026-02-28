using System;
using BoardFlow.Formats.Gerber.Entities.Apertures;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Primitives;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Handling;

namespace BoardFlow.Converters.GerberToSgm;

public class PrimitiveConverter(MacroApertureTemplate template, MacroAperture aperture) {
    private double GetParameterValue(string name) {
        if (template.Expressions.TryGetValue(name, out var expression)) {
            return Calc(expression);
        }

        var num = int.Parse(name.Trim('$'));
        return aperture.ParameterValues[num-1];
    }
    
    private double Calc(IExpression? expression) {
        if (expression == null)
            throw new ApplicationException("Invalid expression");
        switch (expression) {
            case ValueExpression ve:
                return ve.Value;
            case OperationExpression oe:
                var lev = Calc(oe.LeftExpression);
                var rev = Calc(oe.RightExpression);
                return oe.OperationType switch {
                    OperationType.Add => lev + rev,
                    OperationType.Subtract => lev - rev,
                    OperationType.Multiply => lev * rev,
                    OperationType.Divide => lev / rev,
                    _ => throw new ArgumentOutOfRangeException()
                };
            case ParameterExpression pe:
                return GetParameterValue(pe.Name);
            default: throw new Exception("PrimitiveConverter : CalculateExpression");
        }
    }
    
    public Shape Convert(IPrimitive primitive) {
        return primitive switch {
            Circle c => Convert(c),
            _ => throw new Exception("PrimitiveConvert : Convert")
        };
    }

    Shape Convert(Circle circle) {
        
        //var c = new Contour();

        var r = Calc(circle.Diameter) / 2;
        var cx = Calc(circle.CenterX);
        var cy = Calc(circle.CenterY);
        var painter = new Painter<Contour>(cx-r, cy);
        painter.ArcToInc(2*r,0, r, RotationDirection.Clockwise, false);
        painter.ArcToInc(-2*r,0,r, RotationDirection.Clockwise, false);

        return new Shape(painter.Root);
    }

    
    
    Shape Convert(Polygon polygon) {

        var r = Calc(polygon.Diameter) / 2;
        var cx = Calc(polygon.CenterX);
        var cy = Calc(polygon.CenterY);
        var painter = new Painter<Contour>(cx+r, cy);
        var vc = (int)Calc(polygon.VerticesCount);
        var angle = Math.PI * 2 / vc;
        for (var i = 0; i < vc; i++) {
            var curAngle = angle * i;
            var curX = r*Math.Cos(curAngle);
            var curY = r*Math.Sin(curAngle);
            painter.LineToAbs(cx+curX, cy+curY);
        }

        return new Shape(painter.Root);
    }
}