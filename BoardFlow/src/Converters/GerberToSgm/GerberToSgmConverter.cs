/* GerberToSgmConverter.cs */
using System;
using System.Collections.Generic;
using BoardFlow.Formats.Gerber.Entities;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling;
using BoardFlow.Formats.Svg.Entities;
using GraphicElements_Path = BoardFlow.Formats.Sgm.Entities.GraphicElements.Path;

namespace BoardFlow.Converters.GerberToSgm;

public static class GerberToSpvConverter {
    
    public static SvgDocument Convert(GerberDocument document) {
        var result = new SvgDocument { Uom = document.Uom };
        var apertureConverter = new ApertureConverter(document);
        foreach(var operation in document.Operations)
        {
            switch (operation) {
                case PathPaintOperation path:
                    result.Elements.Add(ConvertPath(path));
                    break;
                case FlashOperation flash:
                    var aperture = document.Apertures[flash.ApertureCode];
                    result.Elements.AddRange(apertureConverter.ConvertAperture(flash.Point, aperture));
                    break;
                default:
                    throw new Exception("GerberToSvgConverter: Convert");
            }
        }
        
        return result;
    }
    
    static GraphicElements_Path ConvertPath(PathPaintOperation operation) {
        var result = new GraphicElements_Path {
            StrokeWidth = operation.Aperture.Diameter,
        };
        var startPartPoint = operation.StartPoint;
        foreach (var op in operation.Parts) {
            switch (op) {
                case LinePathPart line:
                    result.Curves.Add(new Line { PointFrom = startPartPoint, PointTo = line.EndPoint });
                    break;
                case ArcPathPart arc:
                    result.Curves.AddRange(ConvertArcPath(startPartPoint, arc, result));
                    startPartPoint = arc.EndPoint;
                    break;
                default:
                    throw new Exception("GerberToSvgConverter: ConvertPath");
            }
        }
        return result;
    }
    
    static List<Arc> ConvertArcPath(Point gsp, ArcPathPart gap, CurvesOwner owner) {
        var result = new List<Arc>();
        
        double cx, cy;
        
        if (gap.QuadrantMode == QuadrantMode.Multi) {
            cx = gsp.X + gap.IOffset;
            cy = gsp.Y + gap.JOffset;
        } else {
            cx = gap.EndPoint.X < gsp.X ? gsp.X - Math.Abs(gap.IOffset) : gsp.X + Math.Abs(gap.IOffset);
            cy = gap.EndPoint.Y < gsp.Y ? gsp.Y - Math.Abs(gap.JOffset) : gsp.Y + Math.Abs(gap.JOffset);
        }
        
        var r1 = Math.Sqrt(Math.Pow(cx - gsp.X, 2) + Math.Pow(cy - gsp.Y, 2));
        var r2 = Math.Sqrt(Math.Pow(cx - gap.EndPoint.X, 2) + Math.Pow(cy - gap.EndPoint.Y, 2));
        var tr = (r2 + r1) / 2; 
        var arcWay = Geometry.ArcWay(gsp, gap.EndPoint, new Point(cx, cy));
        
        if (gsp == gap.EndPoint) {
            var mpx = cx + (cx - gsp.X);
            var mpy = cy + (cy - gsp.Y);
            result.Add(new Arc { RotationDirection = arcWay.RotationDirection, Radius = tr, IsLargeArc = false, PointFrom = gsp, PointTo = new Point(mpx, mpy) });
            result.Add(new Arc { RotationDirection = arcWay.RotationDirection, Radius = tr, IsLargeArc = true, PointFrom = new Point(mpx, mpy), PointTo = gsp });
        } else {
            result.Add(new Arc { RotationDirection = arcWay.RotationDirection, PointTo = gap.EndPoint, Radius = tr, IsLargeArc = arcWay.IsLarge, PointFrom = gsp });
        }
        return result;
    }
}