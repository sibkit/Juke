/* SvgWriter.cs */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling;
using BoardFlow.Formats.Svg.Entities;
using GraphicElements_Path = BoardFlow.Formats.Sgm.Entities.GraphicElements.Path;

namespace BoardFlow.Formats.Svg.Writing;

[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public static class SvgWriter {

    public static Bounds CalculateViewBox(SvgDocument doc) {
        var result = new Bounds(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);
        foreach (var e in doc.Elements) {
            result = result.ExtendBounds(e.Bounds);
        }
        var field = result.GetWidth() > result.GetHeight() ? result.GetWidth() * 0.04 : result.GetHeight() * 0.04;
        return new Bounds(
            result.MinPoint.X - field,
            result.MinPoint.Y - field,
            result.MaxPoint.X + field,
            result.MaxPoint.Y + field
        );
    }

    static string Format(double value)
        => value.ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture);
    
    static void WritePathPart(StreamWriter writer, ICurve curve) {
        switch (curve) {
            case Line l:
                writer.Write($"L{Format(l.PointTo.X)} {Format(l.PointTo.Y)}");
                break;
            case Arc a:
                writer.Write($"A{Format(a.Radius)} {Format(a.Radius)} 0 {(a.IsLargeArc ? "1" : "0")} {(a.RotationDirection == RotationDirection.Clockwise ? "0" : "1")} {Format(a.PointTo.X)} {Format(a.PointTo.Y)}");
                break;
        }
    }

    static void WritePath(StreamWriter writer, GraphicElements_Path path) {
        if(path.Curves.Count == 0) return;
        var sp = path.Curves[0].PointFrom;
        
        writer.Write($"\n<path d=\"M{Format(sp.X)} {Format(sp.Y)}");
        foreach (var pp in path.Curves) WritePathPart(writer, pp);
            
        if (path.StrokeWidth > Geometry.Accuracy) {
            writer.Write($"\" fill=\"none\" stroke-linecap=\"round\" stroke-linejoin=\"round\" stroke-width=\"{Format(path.StrokeWidth)}\"/>");
        } else {
            writer.Write("\" fill=\"none\" stroke-width=\"0.1\"/>");
        }
    }
    
    static void WriteContour(StreamWriter writer, Contour contour) {
        if(contour.Curves.Count == 0) return;
        var sp = contour.Curves[0].PointFrom;
        
        writer.Write($"\n<path d=\"M{Format(sp.X)} {Format(sp.Y)}");
        foreach (var pp in contour.Curves) WritePathPart(writer, pp);
        writer.Write("Z\" stroke=\"none\"/>");
    }

    static void WriteShape(StreamWriter writer, Shape shape) {
        if (shape.OuterContours.Count == 0) return;
        writer.Write("\n<path fill-rule=\"evenodd\" stroke=\"none\" d=\"");
        
        foreach (var oc in shape.OuterContours) {
            if (Contours.GetRotationDirection(oc) != RotationDirection.Clockwise) oc.Reverse();
            var sp = oc.Curves[0].PointFrom;
            writer.Write($"M{Format(sp.X)} {Format(sp.Y)}");
            foreach (var pp in oc.Curves) WritePathPart(writer, pp);
            writer.Write("Z"); 
        }
        foreach (var ic in shape.InnerContours) {
            if (Contours.GetRotationDirection(ic) != RotationDirection.CounterClockwise) ic.Reverse();
            var sp = ic.Curves[0].PointFrom;
            writer.Write($"M{Format(sp.X)} {Format(sp.Y)}");
            foreach (var pp in ic.Curves) WritePathPart(writer, pp);
            writer.Write("Z"); 
        }
        writer.Write("\"/>");
    }

    static void WriteDot(StreamWriter writer, Dot dot) {
        writer.Write($"\n<circle cx=\"{Format(dot.CenterPoint.X)}\" cy=\"{Format(dot.CenterPoint.Y)}\" r=\"{Format(dot.Diameter / 2)}\" stroke=\"none\"/>");
    }

    public static void Write(SvgDocument doc, string fileName, Bounds? forcedViewBox = null) {
        using var swr = new StreamWriter(fileName);
        var vbr = forcedViewBox ?? doc.ViewBox ?? CalculateViewBox(doc);

        double dpi = 96.0;
        double scale = doc.Uom == BoardFlow.Formats.Common.Entities.Uom.Metric ? (dpi / 25.4) : dpi; 

        string widthStr = Format(vbr.GetWidth() * scale) + "px";
        string heightStr = Format(vbr.GetHeight() * scale) + "px";
        
        // Внедряем data-uom, чтобы JS знал оригинальные единицы измерения CAD-системы
        string uomStr = doc.Uom == BoardFlow.Formats.Common.Entities.Uom.Metric ? "Metric" : "Inch";

        swr.Write($"<svg xmlns=\"http://www.w3.org/2000/svg\" data-uom=\"{uomStr}\" width=\"{widthStr}\" height=\"{heightStr}\" viewBox=\"{Format(vbr.MinPoint.X)} {Format(vbr.MinPoint.Y)} {Format(vbr.GetWidth())} {Format(vbr.GetHeight())}\">");
                  
        double translateY = vbr.MinPoint.Y + vbr.MaxPoint.Y;
        swr.Write($"\n<g transform=\"translate(0, {Format(translateY)}) scale(1, -1)\" fill=\"black\" stroke=\"black\">"); 
        
        foreach (var e in doc.Elements) {
            switch (e) {
                case GraphicElements_Path p: WritePath(swr, p); break;
                case Contour c: WriteContour(swr, c); break;
                case Shape sh: WriteShape(swr, sh); break;
                case Dot dot: WriteDot(swr, dot); break;
            }
        }
        swr.Write("\n</g>\n</svg>");
        swr.Flush();
    }
}