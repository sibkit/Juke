using System;
using System.Collections.Generic;
using BoardFlow.Formats.Gerber.Entities;
using BoardFlow.Formats.Gerber.Entities.Apertures;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Handling;

namespace BoardFlow.Converters.GerberToSgm;

public class ApertureConverter(GerberDocument document) {

    public List<Shape> ConvertAperture(Point coordinate, IAperture aperture) {

        return aperture switch {
            CircleAperture circle => [ConvertCircleAperture(coordinate, circle)],
            RectangleAperture rect => [ConvertRectangleAperture(coordinate, rect)],
            MacroAperture macro => ConvertMacroAperture(coordinate, macro),
            ObRoundAperture obRound => [ConvertObRoundAperture(coordinate, obRound)],
            // case PolygonAperture polygon:
            //     break;
            _ => throw new Exception("Unknown aperture type")
        };
    }

    public Shape ConvertCircleAperture(Point coordinate, CircleAperture circle) {
        var r = circle.Diameter / 2;

        
        var pOuter = new Painter<Contour>(coordinate.X - r, coordinate.Y);
        pOuter.ArcToInc(2 * r, 0, r, RotationDirection.CounterClockwise, false);
        pOuter.ArcToInc(-2 * r, 0, r, RotationDirection.CounterClockwise, false);
        var result = new Shape(pOuter.Root);


        if (circle.HoleDiameter is { } hd and > 0.000001) {
            var pInner = new Painter<Contour>(coordinate.X - r, coordinate.Y);
            pInner.ArcToInc(2 * r, 0, r, RotationDirection.Clockwise, false);
            pInner.ArcToInc(-2 * r, 0, r, RotationDirection.Clockwise, false);
            result.InnerContours.Add(pInner.Root);
        }

        return result;
    }

    public Shape ConvertRectangleAperture(Point coordinate, RectangleAperture rect) {

        var outerPainter = new Painter<Contour>(coordinate.X - rect.XSize / 2, coordinate.Y - rect.YSize / 2);
        outerPainter.LineToInc(rect.XSize, 0);
        outerPainter.LineToInc(0, rect.YSize);
        outerPainter.LineToInc(-rect.XSize, 0);
        outerPainter.LineToInc(0, -rect.YSize);
        
        var result = new Shape( outerPainter.Root);
        
        if (rect.HoleDiameter is { } hd and > 0.000001) {
            var r = hd / 2;
            var innerPainter = new Painter<Contour>(coordinate.X - r, coordinate.Y);
            innerPainter.ArcToInc(2 * r, 0, r, RotationDirection.Clockwise, false);
            innerPainter.ArcToInc(-2 * r, 0, r, RotationDirection.Clockwise, false);
            result.InnerContours.Add(innerPainter.Root);
        }

        return result;
    }

    public Shape ConvertObRoundAperture(Point coordinate, ObRoundAperture obRound) {
        Shape result;

        if (obRound.XSize >= obRound.YSize) {
            var br = obRound.YSize / 2;
            var outerPainter = new Painter<Contour>(coordinate.X - br, coordinate.Y - obRound.YSize / 2 + br);
            outerPainter.ArcToInc(2 * br, 0, br, RotationDirection.CounterClockwise, false);
            outerPainter.LineToInc(0, obRound.YSize - 2 * br);
            outerPainter.ArcToInc(-2 * br, 0, br, RotationDirection.CounterClockwise, false);
            outerPainter.LineToInc(0, -(obRound.YSize - 2 * br));
            result = new Shape( outerPainter.Root);
        } else {
            var br = obRound.XSize / 2;
            var outerPainter = new Painter<Contour>(coordinate.X - (obRound.XSize / 2 - br), coordinate.Y + br);
            outerPainter.ArcToInc(0, -2 * br, br, RotationDirection.CounterClockwise, false);
            outerPainter.LineToInc(obRound.XSize - 2 * br, 0);
            outerPainter.ArcToInc(0, 2 * br, br, RotationDirection.CounterClockwise, false);
            outerPainter.LineToInc(-(obRound.XSize - 2 * br), 0);
            result = new Shape(outerPainter.Root);
        }

        if (obRound.HoleDiameter is { } hd and > 0.000001) {
            var r = hd / 2;
            var innerPainter = new Painter<Contour>(coordinate.X - r, coordinate.Y);
            innerPainter.ArcToInc(r, r, r, RotationDirection.Clockwise, false);
            innerPainter.ArcToInc(r, -r, r, RotationDirection.Clockwise, false);
            innerPainter.ArcToInc(-r, -r, r, RotationDirection.Clockwise, false);
            innerPainter.ArcToInc(-r, r, r, RotationDirection.Clockwise, false);
            result.InnerContours.Add(innerPainter.Root);
        }

        return result;
    }

    public List<Shape> ConvertMacroAperture(Point coordinate, MacroAperture macro) {
        var result = new List<Shape>();
        if (!document.MacroApertureTemplates.TryGetValue(macro.TemplateName, out var template))
            throw new ApplicationException("Не найден шаблон для макроаппертуры: \"" + macro.TemplateName + "\"");

        var shapes = (from primitive in template.Primitives let pc = new PrimitiveConverter(template, macro) select pc.Convert(primitive)).ToList();

        Console.WriteLine("ApertureConverter.ConvertMacroAperture: Macro apertures not implemented");
        // result.StartPoint = coordinate - new Point(rect.XSize/2, rect.YSize/2);
        
        return result;
    }
}