using System;
using System.Collections.Generic;
using System.Linq;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;

namespace BoardFlow.Formats.Sgm.Handling.ContourWalkers;

public class ContoursWalker {
        
    private readonly Dictionary<Point, List<TransitionPoint>> _pointsMap = new ();
    private readonly List<ICurve> _placedCurves = [];
    //private readonly RotationDirection _rotationDirection;
    
    public Contour Contour1 { get; }
    public Contour Contour2 { get; }

    public ContoursWalker(Contour contour1, Contour contour2) {

        Contour1 = Contours.SplitByRelationPoints(contour1, contour2);
        Contour2 = Contours.SplitByRelationPoints(contour2, contour1);

        //_rotationDirection = Contours.GetRotationDirection(Contour1);
        
        // if (_rotationDirection == Contours.GetRotationDirection(Contour2))
        //     Contour2 = Contours.GetReversed(Contour2);
        
        FillPointsMap(Contour1);
        FillPointsMap(Contour2);
    }
    
    private void FillPointsMap(Contour contour) {
        for (var i = 0; i < contour.Curves.Count; i++) {
            var outCurve = contour.Curves[i];
            var inCurve = i > 0 ? contour.Curves[i-1]:contour.Curves.Last();
            var rp = Geometry.RoundPoint(outCurve.PointFrom);
            if(!_pointsMap.ContainsKey(rp))
                _pointsMap.Add(rp, []);
            _pointsMap[rp].Add(new TransitionPoint {
                Contour = contour,
                InCurve = inCurve,
                OutCurve = outCurve
            });
        }
    }

    // private ICurve FindOuterStartCurve() {
    //     Point? firstPoint = null;
    //     var minX = double.PositiveInfinity;
    //     foreach (var kvp in _pointsMap) {
    //         if (kvp.Key.X < minX) {
    //             minX = kvp.Key.X;
    //             firstPoint = kvp.Key;
    //         }
    //     }
    //
    //     if (firstPoint == null)
    //         throw new Exception("ContoursWalker: WalkMerge");
    //
    //     var points = _pointsMap[firstPoint.Value];
    //     var angle = double.PositiveInfinity;
    //     ICurve result = null;
    //     foreach (var tp in points) {
    //         var a = Angles.PositiveNormalize(Angles.CalculateAngle(firstPoint.Value.WithNewY(firstPoint.Value.Y-1), tp.InCurve.PointFrom, firstPoint.Value));
    //         if (a < angle) {
    //             angle = a;
    //             result = tp.InCurve;
    //         }
    //     }
    //     return result;
    // }
    
    private static ICurve GetExtremeCurve(ICurve inCurve, ICurve outCurve1, ICurve outCurve2, RotationDirection direction) {

        var vecOut1 = Curves.GetTangentInVector(outCurve1);
        var vecOut2 = Curves.GetTangentInVector(outCurve2);
        var vecIn = Curves.GetTangentOutVector(inCurve);

        var a1 = Angles.PiNormalize(Vectors.Angle(vecIn, vecOut1));
        var a2 = Angles.PiNormalize(Vectors.Angle(vecIn, vecOut2));

        if (Math.Abs(a1 - (-Math.PI)) < Geometry.Accuracy) {
            a1 = Math.PI;
        }
        
        if (Math.Abs(a2 - (-Math.PI)) < Geometry.Accuracy) {
            a2 = Math.PI;
        }
        
        if (Math.Abs(a1 - a2) > Geometry.Accuracy) {
            return direction switch {
                RotationDirection.Clockwise => a1 > a2 ? outCurve1 : outCurve2,
                RotationDirection.CounterClockwise => a1 < a2 ? outCurve1 : outCurve2,
                _ => throw new Exception("ContoursWalker:GetExtremeCurve(Invalid rotation direction)")
            };
        }
        // if (Math.Abs(a1 - a2) > Math.PI - Geometry.Accuracy) {
        //     //разворот
        //     return direction switch {
        //         RotationDirection.Clockwise => a1 > a2 ? outCurve1 : outCurve2,
        //         RotationDirection.CounterClockwise => a1 < a2 ? outCurve1 : outCurve2,
        //         _ => throw new Exception("ContoursWalker:GetExtremeCurve(Invalid rotation direction)")
        //     };
        // }
        
        var ctx1 = Curves.GetCurvature(outCurve1, 0);
        var ctx2 = Curves.GetCurvature(outCurve2, 0);

        return direction switch {
            RotationDirection.Clockwise => ctx1.Direction switch {
                RotationDirection.CounterClockwise when ctx2.Direction == RotationDirection.CounterClockwise => ctx1.Value > ctx2.Value ? outCurve2 : outCurve1,
                RotationDirection.CounterClockwise => outCurve1,
                RotationDirection.None => ctx2.Direction == RotationDirection.CounterClockwise ? outCurve2 : outCurve1,
                RotationDirection.Clockwise when ctx2.Direction != RotationDirection.Clockwise => outCurve2,
                RotationDirection.Clockwise => ctx1.Value > ctx2.Value ? outCurve1 : outCurve2,
                _ => throw new Exception("ContoursWalker:GetExtremeCurve(1)")
            },
            RotationDirection.CounterClockwise => ctx1.Direction switch {
                RotationDirection.Clockwise when ctx2.Direction == RotationDirection.Clockwise => ctx1.Value > ctx2.Value ? outCurve1 : outCurve2,
                RotationDirection.Clockwise => outCurve1,
                RotationDirection.None => ctx2.Direction == RotationDirection.Clockwise ? outCurve2 : outCurve1,
                RotationDirection.CounterClockwise when ctx2.Direction != RotationDirection.CounterClockwise => outCurve2,
                RotationDirection.CounterClockwise => ctx1.Value > ctx2.Value ? outCurve2 : outCurve1,
                _ => throw new Exception("ContoursWalker:GetExtremeCurve(2)")
            },
            _ => throw new Exception("ContoursWalker:GetExtremeCurve(3)")
        };
    }

    // private ICurve GetExtremeCurve(IEnumerable<ICurve> curves, RotationDirection direction) {
    //     ICurve extremeCurve = null;
    //     foreach (var curve in curves) {
    //         extremeCurve ??= curve;
    //         
    //     }
    //     return extremeCurve;
    // }
    
    private ICurve NextCurve(ICurve inCurve, RotationDirection contoursRd) {
        var points = _pointsMap[Geometry.RoundPoint(inCurve.PointTo)];
        if (points == null || points.Count == 0)
            throw new Exception("ContoursWalker: NextCurve");

        if (points.Count == 1)
            return points[0].OutCurve;
        

        ICurve result = null;
        foreach (var tp in points) {
            if (_placedCurves.Contains(tp.OutCurve))
                continue;
            if (result == null) {
                result = tp.OutCurve;
                continue;
            }
            result = GetExtremeCurve(inCurve, result, tp.OutCurve, contoursRd);
        }

        return result;
    }

    private Contour WalkFrom(ICurve fromCurve, RotationDirection rd) {
        if (_placedCurves.Contains(fromCurve))
            return null;
        var visitedCurves = new List<ICurve>();
        var prevCurve = fromCurve;
        ICurve firstCurve = null;
        while (true) {
            visitedCurves.Add(prevCurve);
            var curve = NextCurve(prevCurve, rd);
            if(_placedCurves.Contains(curve))
                break;
            if (visitedCurves.Contains(curve)) {
                
                firstCurve = curve;
                break;
            }
            prevCurve = curve;
        }

        if (firstCurve != null) {
            var c = new Contour();
            c.Curves.AddRange(visitedCurves.Skip(visitedCurves.IndexOf(firstCurve)));
            _placedCurves.AddRange(c.Curves);
            return c;
        }
        return null;
    }

    public List<Contour> Walk(RotationDirection rd) {

        //var outerContour = WalkFrom(FindOuterStartCurve());
        //var outerContour = WalkFrom(Contour1.Curves[0], rd);
        
        var allCurves = Contour1.Curves.Union(Contour2.Curves);
        var contours = new List<Contour>();
        foreach (var curve in allCurves) {
            if (!_placedCurves.Contains(curve)) {
                var rc = WalkFrom(curve, rd);
                if(rc != null)
                    contours.Add(rc);
            }
        }
        
        //contours = allCurves.Select(crv => WalkFrom(crv, rd)).Where(c => c != null).ToList();

        // var contours = Contour1.Curves
        //     .Union(Contour2.Curves)
        //     .Select(WalkFrom)
        //     .Where(c => c != null)
        //     .ToList();

        // var result = new Shape(outerContour);
        // result.InnerContours.AddRange(contours.Where(c => {
        //     var rd = Contours.GetRotationDirection(c);
        //     return rd == _rotationDirection.Invert();
        // }));

        return contours;
    }
}