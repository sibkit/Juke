using System;
using System.Collections.Generic;
using System.Linq;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling.ContourWalkers;
using BoardFlow.Formats.Sgm.Handling.Relations;

namespace BoardFlow.Formats.Sgm.Handling;

public static class Contours {


    private static void SplitArcs(Contour contour) {
        for (var i = 0; i < contour.Curves.Count; i++) {
            switch (contour.Curves[i]) {
                case Line:
                    continue;
                case Arc arc:
                    var cp = Geometry.ArcCenter(arc);
                    var rd = arc.RotationDirection;
                    var qStart = Quadrants.GetQuadrant(arc.PointFrom.X - cp.X, arc.PointFrom.Y - cp.Y);
                    var qEnd = Quadrants.GetQuadrant(arc.PointTo.X - cp.X, arc.PointTo.Y - cp.Y);


                    
                    if ((qStart == qEnd) && !arc.IsLargeArc)
                        continue;
                    
                    switch (qStart) {
                        case Quadrant.I_II:
                            qStart = rd == RotationDirection.CounterClockwise ? Quadrant.II : Quadrant.I;
                            break;
                        case Quadrant.II_III:
                            qStart = rd == RotationDirection.CounterClockwise ? Quadrant.III : Quadrant.II;
                            break;
                        case Quadrant.III_IV:
                            qStart = rd == RotationDirection.CounterClockwise ? Quadrant.IV : Quadrant.III;
                            break;
                        case Quadrant.IV_I:
                            qStart = rd == RotationDirection.CounterClockwise ? Quadrant.I : Quadrant.IV;
                            break;
                    }
                    
                    switch (qEnd) {
                        case Quadrant.I_II:
                            qEnd = rd == RotationDirection.CounterClockwise ? Quadrant.I : Quadrant.II;
                            break;
                        case Quadrant.II_III:
                            qEnd = rd == RotationDirection.CounterClockwise ? Quadrant.II : Quadrant.III;
                            break;
                        case Quadrant.III_IV:
                            qEnd = rd == RotationDirection.CounterClockwise ? Quadrant.III : Quadrant.IV;
                            break;
                        case Quadrant.IV_I:
                            qEnd = rd == RotationDirection.CounterClockwise ? Quadrant.IV : Quadrant.I;
                            break;
                    }
                    
                    var qCur = qStart;

                    var splitPoints = new List<Point>(4);
                    
                    while (qCur != qEnd) {
                        var qNew = rd == RotationDirection.CounterClockwise ? qCur.Next() : qCur.Prev();
                        var qt = Quadrants.GetTransitions(qCur, qNew, rd);
                        switch (qt) {
                            case QuadrantTransition.I_II:
                                splitPoints.Add(new Point(cp.X, cp.Y + arc.Radius));
                                break;
                            case QuadrantTransition.II_III:
                                splitPoints.Add(new Point(cp.X - arc.Radius, cp.Y));
                                break;
                            case QuadrantTransition.III_IV:
                                splitPoints.Add(new Point(cp.X, cp.Y - arc.Radius));
                                break;
                            case QuadrantTransition.IV_I:
                                splitPoints.Add(new Point(cp.X + arc.Radius, cp.Y));
                                break;
                            default:
                                throw new Exception("Unexpected QuadrantTransition");
                        }
                        qCur = qNew;
                    } 
                    
                    contour.Curves.RemoveAt(i);
                    var arcs = new List<Arc>(4);
                    foreach (var splitPoint in splitPoints) {
                        arcs.Add(new Arc {
                            Radius = arc.Radius,
                            RotationDirection = rd,
                            IsLargeArc = arc.IsLargeArc,
                            PointFrom = arcs.Count == 0 ? arc.PointFrom : arcs.Last().PointTo,
                            PointTo = splitPoint
                        });
                    }
                    arcs.Add(new Arc {
                        Radius = arc.Radius,
                        RotationDirection = rd,
                        IsLargeArc = arc.IsLargeArc,
                        PointFrom = arcs.Count == 0 ? arc.PointFrom : arcs.Last().PointTo,
                        PointTo = arc.PointTo
                    });
                    contour.Curves.InsertRange(i, arcs);
                    i+=arcs.Count-1;
                    break;
            }
        }
    }


    public static Contour SplitByRelationPoints(Contour contour1, Contour contour2) {
        if (!contour1.Bounds.IsIntersected(contour2.Bounds))
            return contour1;

        var result = new Contour();

        //находим все пересечения
        foreach (var curve in contour1.Curves) {
            var contactPoints = new List<ContactPoint>();
            foreach (var curve2 in contour2.Curves) {
                var relation = RelationManager.DefineRelation(curve, curve2);
                switch (relation) {
                    case NotRelation:
                        break;
                    case ContactRelation ir:
                        contactPoints.AddRange(ir.Points);
                        break;
                    case OverlappingRelation or:
                        contactPoints.AddRange(or.Points);
                        break;
                    default:
                        throw new Exception("Contours: SplitByRelationPoints (Unknown Relation type: " + relation + ")");
                }
            }

            contactPoints.Sort((x, x1) => x.T.CompareTo(x1.T));
            switch (curve) {
                case Line line:
                    var sp = line.PointFrom;
                    foreach (var cp in contactPoints) {
                        if (sp != cp.Point) {
                            result.Curves.Add(new Line {
                                PointFrom = sp,
                                PointTo = cp.Point,
                            });
                            sp = cp.Point;
                        }
                    }

                    if (sp != line.PointTo)
                        result.Curves.Add(new Line {
                            PointFrom = sp,
                            PointTo = line.PointTo,
                        });
                    break;
                case Arc arc:
                    var sp2 = arc.PointFrom;
                    foreach (var cp in contactPoints) {
                        if (sp2 != cp.Point) {
                            result.Curves.Add(new Arc {
                                PointFrom = sp2,
                                PointTo = cp.Point,
                                Radius = arc.Radius,
                                RotationDirection = arc.RotationDirection,
                                IsLargeArc = arc.IsLargeArc
                            });
                        }
                        sp2 = cp.Point;
                    }

                    if (sp2 != arc.PointTo) {
                        result.Curves.Add(new Arc {
                            PointFrom = sp2,
                            PointTo = arc.PointTo,
                            Radius = arc.Radius,
                            RotationDirection = arc.RotationDirection,
                            IsLargeArc = arc.IsLargeArc
                        });
                    }

                    break;
                default:
                    throw new Exception("Contours: Merge (Unknown Curve type: " + curve + ")");
            }
        }
        return result;
    }
    
    private static double RotationAngle(ICurve curLine, ICurve prevLine) {
        return Vectors.Angle(
            new Vector(prevLine.PointTo.X - prevLine.PointFrom.X, prevLine.PointTo.Y - prevLine.PointFrom.Y),
            new Vector(curLine.PointTo.X - curLine.PointFrom.X, curLine.PointTo.Y - curLine.PointFrom.Y));
    }
    
    public static bool CheckContourCurvesPoints(Contour contour) {
        var prevEnd = contour.Curves[^1].PointTo;
        foreach (var curve in contour.Curves) {
            if (prevEnd == curve.PointFrom) {
                prevEnd = curve.PointTo;
            } else {
                return false;
            }
        }
        return true;
    }
    
    public static RotationDirection GetRotationDirection(Contour contour) {
        
        if (contour.Curves.Count < 2)
            throw new ApplicationException("Контур состоит менее чем из 2-х частей");
        contour = contour.Clone();
        SplitArcs(contour);

        var prevCurve = contour.Curves[^1];
        var resultAngle = 0d;
        
        foreach (var curve in contour.Curves) {
            var ra = RotationAngle(curve, prevCurve);
            resultAngle += ra;
            prevCurve = curve;
        }


        if (Math.Abs(resultAngle - Math.PI * 2) < Geometry.Accuracy)
            return RotationDirection.CounterClockwise;
        if (Math.Abs(resultAngle + Math.PI * 2) < Geometry.Accuracy)
            return RotationDirection.Clockwise;
        if (Math.Abs(resultAngle) < Geometry.Accuracy)
            return RotationDirection.None;
        throw new Exception("Contours: GetRotationDirection(2)");
    }

    public static Contour GetReversed(Contour contour) {
        var result = new Contour();
        for (var i = contour.Curves.Count - 1; i >= 0; i--) {
            var part = contour.Curves[i];
            result.Curves.Add(part.GetReversed());
        }
        return result;
    }

    // public static Contour Move(Contour contour, double dx, double dy) {
    //     var result = new Contour();
    //     foreach (var curve in contour.Curves) {
    //         result.Curves.Add(Curves.Move(curve, dx, dy));
    //     }
    //     return result;
    // }
    

    public static Shape Union(Contour contour1, Contour contour2) {
        
        var rd = GetRotationDirection(contour1);
        
        if (rd != GetRotationDirection(contour2))
            contour2 = GetReversed(contour2);

        var contours = new ContoursWalker(contour1, contour2).Walk(rd);
        
        var followingContours = new List<Contour>();
        var reverseContours = new List<Contour>();

        foreach (var contour in contours) {
            var crd = GetRotationDirection(contour);
            if(crd == rd)
                followingContours.Add(contour);
            else if(crd == rd.Invert()) {
                reverseContours.Add(contour);
            }
        }

        var result = new Shape(followingContours[0]);
        result.InnerContours.AddRange(reverseContours);
        return result;
    }

    public static Shape Subtract(Contour contour1, Contour contour2) {
        var rd = GetRotationDirection(contour1);
        
        if (rd == GetRotationDirection(contour2))
            contour2 = GetReversed(contour2);

        var contours = new ContoursWalker(contour1, contour2).Walk(rd);
        
        var followingContours = new List<Contour>();
        var reverseContours = new List<Contour>();

        foreach (var contour in contours) {
            var crd = GetRotationDirection(contour);
            if(crd == rd)
                followingContours.Add(contour);
            else if(crd == rd.Invert()) {
                reverseContours.Add(contour);
            }
        }

        if (followingContours.Count == 0)
            return null;
        
        var result = new Shape(followingContours[0]);
        result.InnerContours.AddRange(reverseContours);
        return result;
    }
    
    public static Shape Intersect(Contour contour1, Contour contour2) {
        return null;
    }
    
    public static Shape Exclude(Contour contour1, Contour contour2) {
        return null;
    }
}

