using System;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Formats.Sgm.Handling;

public struct Curvature {
    public double Value { get; init; }
    public RotationDirection Direction { get; init; }
}

public static class Curves {
    public static Vector GetTangentInVector(ICurve curve) {

        switch (curve) {
            case Line line:
                return new Vector(line.PointTo.X - line.PointFrom.X, line.PointTo.Y - line.PointFrom.Y);
            case Arc arc:
                var cp = Geometry.ArcCenter(arc);
                var vecA = new Vector(arc.PointFrom.X - cp.X, arc.PointFrom.Y - cp.Y);
                var qA = Quadrants.GetQuadrant(vecA.X, vecA.Y);

                double cwbX = 0;

                switch (qA) {
                    case Quadrant.I:
                    case Quadrant.II:
                        cwbX = 1;
                        break;
                    case Quadrant.III:
                    case Quadrant.IV:
                        cwbX = -1;
                        break;
                    case Quadrant.I_II:
                        return arc.RotationDirection == RotationDirection.Clockwise ? new Vector(1, 0) : new Vector(-1, 0);
                    case Quadrant.II_III:
                        return arc.RotationDirection == RotationDirection.Clockwise ? new Vector(0, 1) : new Vector(0, -1);
                    case Quadrant.III_IV:
                        return arc.RotationDirection == RotationDirection.Clockwise ? new Vector(-1, 0) : new Vector(1, 0);
                    case Quadrant.IV_I:
                        return arc.RotationDirection == RotationDirection.Clockwise ? new Vector(0, -1) : new Vector(0, 1);
                }

                var bx = arc.RotationDirection == RotationDirection.Clockwise ? cwbX! : -cwbX!;
                var by = -1d * vecA.X * bx / vecA.Y;
                return new Vector(bx, by);
            default:
                throw new Exception("Unknown curve type");

        }
    }

    public static Vector GetTangentOutVector(ICurve curve) {

        switch (curve) {
            case Line line:
                return new Vector(line.PointTo.X - line.PointFrom.X, line.PointTo.Y - line.PointFrom.Y);
            case Arc arc:
                var cp = Geometry.ArcCenter(arc);
                var vecA = new Vector(arc.PointTo.X - cp.X, arc.PointTo.Y - cp.Y);
                var qA = Quadrants.GetQuadrant(vecA.X, vecA.Y);

                double cwbX = 0;

                switch (qA) {
                    case Quadrant.I:
                    case Quadrant.II:
                        cwbX = 1;
                        break;
                    case Quadrant.III:
                    case Quadrant.IV:
                        cwbX = -1;
                        break;
                    case Quadrant.I_II:
                        return arc.RotationDirection == RotationDirection.Clockwise ? new Vector(1, 0) : new Vector(-1, 0);
                    case Quadrant.II_III:
                        return arc.RotationDirection == RotationDirection.Clockwise ? new Vector(0, 1) : new Vector(0, -1);
                    case Quadrant.III_IV:
                        return arc.RotationDirection == RotationDirection.Clockwise ? new Vector(-1, 0) : new Vector(1, 0);
                    case Quadrant.IV_I:
                        return arc.RotationDirection == RotationDirection.Clockwise ? new Vector(0, -1) : new Vector(0, 1);
                }

                var bx = arc.RotationDirection == RotationDirection.Clockwise ? cwbX! : -cwbX!;
                var by = -1d * vecA.X * bx / vecA.Y;
                return new Vector(bx, by);
            default:
                throw new Exception("Unknown curve type");

        }
    }

    public static Curvature GetCurvature(ICurve curve, double t) {
        var value = curve switch {
            Line line => 0,
            Arc arc => 1d / arc.Radius,
            _ => throw new Exception("Unknown curve type")
        };
        var direction = curve switch {
            Line line => RotationDirection.None,
            Arc arc => arc.RotationDirection,
            _ => throw new Exception("Unknown curve type (2)")
        };
        return new Curvature {
            Value = value,
            Direction = direction,
        };
    }
    
    // public static ICurve Move(ICurve curve, double dx, double dy) {
    //     return curve switch {
    //         Arc arc => new Arc {
    //             PointTo = arc.PointTo.Move(dx, dy),
    //             PointFrom = arc.PointFrom.Move(dx, dy),
    //             Radius = arc.Radius,
    //             IsLargeArc = arc.IsLargeArc,
    //             RotationDirection = arc.RotationDirection
    //         },
    //         Line line => new Line {
    //             PointTo = line.PointTo.Move(dx, dy),
    //             PointFrom = line.PointFrom.Move(dx, dy),
    //         },
    //         _ => throw new Exception(nameof(curve))
    //     };
    // }
}