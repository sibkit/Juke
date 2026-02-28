using System;
using System.Collections.Generic;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling.Relations.PointsSearch;

namespace BoardFlow.Formats.Sgm.Handling.Relations;

public static class RelationManager {
    
    private static readonly CircleCirclePointsFinder CircleCircleIPF = new();
    private static readonly LineLinePointsFinder LineLineIPF = new();
    private static readonly LineCirclePointsFinder LineCircleIPF = new();

    static bool InRange(double t) {
        return t > -1*Geometry.Accuracy && t < 1d + Geometry.Accuracy;
    }
    
    public static IRelation DefineRelation(ICurve curve, ICurve baseCurve) {
        if (!curve.Bounds.IsIntersected(baseCurve.Bounds))
            return new NotRelation();
        
        var contactPoints = curve switch {
            Line part1 when baseCurve is Line part2 => LineLineIPF.FindContactPoints(part1, part2),
            Arc part1 when baseCurve is Arc part2 => CircleCircleIPF.FindContactPoints(part1, part2),
            Arc part1 when baseCurve is Line part2 => LineCircleIPF.FindContactPoints(part2, part1),
            Line part1 when baseCurve is Arc part2 => LineCircleIPF.FindContactPoints(part1, part2),
            _ => throw new Exception("Intersector: FindIntersections => Cannot define segment(s)")
        };


        if (contactPoints.isMatch) {
            var t1 = TCalculator.CalculateT(curve, baseCurve.PointFrom);
            var t2 = TCalculator.CalculateT(curve, baseCurve.PointTo);
            var bt1 = TCalculator.CalculateT(baseCurve, curve.PointFrom);
            var bt2 = TCalculator.CalculateT(baseCurve, curve.PointTo);
            var points = new List<ContactPoint>();
            
            if(InRange(t1)){ points.Add(new ContactPoint {
                    Point = baseCurve.PointFrom,
                    T = t1,
                    BaseT = 0,
                    Curve = curve,
                    BaseCurve = baseCurve
                });}
            if(InRange(t2)) points.Add(new ContactPoint {
                    Point = baseCurve.PointTo,
                    T = t2,
                    BaseT = 1,
                    Curve = curve,
                    BaseCurve = baseCurve
                });
            if (InRange(bt1) && curve.PointFrom != baseCurve.PointFrom && curve.PointFrom != baseCurve.PointTo)
                points.Add(new ContactPoint {
                    Point = curve.PointFrom,
                    T = 0,
                    BaseT = bt1,
                    Curve = curve,
                    BaseCurve = baseCurve
                });
            if (InRange(bt2) && curve.PointTo != baseCurve.PointTo && curve.PointTo != baseCurve.PointFrom)
                points.Add(new ContactPoint {
                    Point = curve.PointTo,
                    T = 1,
                    BaseT = bt2,
                    Curve = curve,
                    BaseCurve = baseCurve
                });
            points.Sort((a, b) => a.T.CompareTo(b.T));
            return new OverlappingRelation {
                Points = points
            };

        } else {
            var points = new List<ContactPoint>();
            foreach (var pt in contactPoints.points) {
                var t1 = TCalculator.CalculateT(curve, pt);
                var t2 = TCalculator.CalculateT(baseCurve, pt);
                if (t1 > 0 && t1 <= 1 && t2 > 0 && t2 <= 1) {
                    points.Add(new ContactPoint {
                        T = t1,
                        Point = pt,
                        BaseT = t2,
                        Curve = curve,
                        BaseCurve = baseCurve
                    });
                }
            }
        
            if (points.Count == 0) 
                return new NotRelation();
        
            points.Sort((a, b) => a.T.CompareTo(b.T));
            return new ContactRelation {
                Points = points
            };
        }
    }
}