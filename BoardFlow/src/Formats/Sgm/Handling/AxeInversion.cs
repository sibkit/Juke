using System;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Formats.Sgm.Handling;

public static class AxeInversionExtension {
    public static void InvertYAxe(this SgmDocument document) {
        foreach (var e in document.GraphicElements) {
            switch (e) {

                case CurvesOwner ctr:
                    ctr.InvertYAxe();
                    break;
                case Shape shape:
                    foreach (var oc in shape.OuterContours)
                        oc.InvertYAxe();
                    foreach (var ic in shape.InnerContours)
                        ic.InvertYAxe();
                    break;
                case Dot dot:
                    dot.CenterPoint = new Point(dot.CenterPoint.X, -1*dot.CenterPoint.Y);
                    break;
                default:
                    throw new Exception("GerberToSvgConverter: InvertAxis");
            }
            //e.UpdateBounds();
        }
    }

    private static void InvertYAxe(this ICurve curve) {

        switch (curve) {
            case Line line:
                line.PointFrom = new Point(line.PointFrom.X, -1*line.PointFrom.Y);
                line.PointTo = new Point(line.PointTo.X, -1*line.PointTo.Y);
                break;
            case Arc arc:
                arc.PointFrom = new Point(arc.PointFrom.X, -1*arc.PointFrom.Y);
                arc.PointTo = new Point(arc.PointTo.X, -1*arc.PointTo.Y);
                arc.RotationDirection = arc.RotationDirection.Invert();
                break;
            default:
                throw new Exception("Area: InvertAxis");
        }
    }

    private static void InvertYAxe(this CurvesOwner ctx) {
        //ctx.StartPoint = ctx.StartPoint.WithNewY(-ctx.StartPoint.Y);
        foreach (var p in ctx.Curves) {
            p.InvertYAxe();
        }
    }
}