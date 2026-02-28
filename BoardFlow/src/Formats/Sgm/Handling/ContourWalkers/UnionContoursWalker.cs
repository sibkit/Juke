// using BoardFlow.Spv.Entities;
// using BoardFlow.Spv.Entities.GraphicElements;
//
// namespace BoardFlow.Spv.Handling.ContourWalkers;
//
// public class UnionContoursWalker {
//     
//     private readonly Dictionary<Point, List<TransitionPoint>> _pointsMap = new ();
//     private readonly List<ICurve> _placedCurves = [];
//     private readonly RotationDirection _rotationDirection;
//     
//     public Contour Contour1 { get; }
//     public Contour Contour2 { get; }
//
//     public UnionContoursWalker(Contour contour1, Contour contour2) {
//
//         Contour1 = Contours.SplitByRelationPoints(contour1, contour2);
//         Contour2 = Contours.SplitByRelationPoints(contour2, contour1);
//
//         _rotationDirection = Contours.GetRotationDirection(Contour1);
//         
//         if (_rotationDirection != Contours.GetRotationDirection(Contour2))
//             Contour2 = Contours.GetReversed(Contour2);
//         
//         FillPointsMap(Contour1);
//         FillPointsMap(Contour2);
//     }
//     
//     private void FillPointsMap(Contour contour) {
//         for (var i = 0; i < contour.Curves.Count; i++) {
//             var outCurve = contour.Curves[i];
//             var inCurve = i > 0 ? contour.Curves[i-1]:contour.Curves.Last();
//             var rp = Geometry.RoundPoint(outCurve.PointFrom);
//             if(!_pointsMap.ContainsKey(rp))
//                 _pointsMap.Add(rp, []);
//             _pointsMap[rp].Add(new TransitionPoint {
//                 Contour = contour,
//                 InCurve = inCurve,
//                 OutCurve = outCurve
//             });
//         }
//     }
//
//     private ICurve FindOuterStartCurve() {
//         Point? firstPoint = null;
//         var minX = double.PositiveInfinity;
//         foreach (var kvp in _pointsMap) {
//             if (kvp.Key.X < minX) {
//                 minX = kvp.Key.X;
//                 firstPoint = kvp.Key;
//             }
//         }
//
//         if (firstPoint == null)
//             throw new Exception("ContoursWalker: WalkMerge");
//
//         var points = _pointsMap[firstPoint.Value];
//         var angle = double.PositiveInfinity;
//         ICurve result = null;
//         foreach (var tp in points) {
//             var a = Angles.PositiveNormalize(Angles.CalculateAngle(firstPoint.Value.WithNewY(firstPoint.Value.Y-1), tp.InCurve.PointFrom, firstPoint.Value));
//             if (a < angle) {
//                 angle = a;
//                 result = tp.InCurve;
//             }
//         }
//         return result;
//     }
//
//     private ICurve NextCurve(ICurve inCurve, RotationDirection contoursRd) {
//         var points = _pointsMap[Geometry.RoundPoint(inCurve.PointTo)];
//         if (points == null || points.Count == 0) 
//             throw new Exception("ContoursWalker: NextCurve");
//         
//         if(points.Count==1)
//             return points[0].OutCurve;
//
//         var angle = double.PositiveInfinity;
//         ICurve result = null;
//         foreach (var tp in points) {
//             if(_placedCurves.Contains(tp.OutCurve))
//                 continue;
//
//             var startCurveVec = Curves.GetCurveInVector(tp.OutCurve);
//             var pointTo = new Point(inCurve.PointTo.X+startCurveVec.X, inCurve.PointTo.Y+startCurveVec.Y);
//             
//             var a = contoursRd == RotationDirection.CounterClockwise ?
//                 Angles.PositiveNormalize(Angles.CalculateAngle(inCurve.PointFrom, pointTo, inCurve.PointTo)) :
//                 Angles.PositiveNormalize(Angles.CalculateAngle(pointTo, inCurve.PointFrom, inCurve.PointTo));
//             if (a < angle) {
//                 angle = a;
//                 result = tp.OutCurve;
//             }
//         }
//         return result;
//     }
//
//     public Contour WalkFrom(ICurve fromCurve) {
//         if (_placedCurves.Contains(fromCurve))
//             return null;
//         var visitedCurves = new List<ICurve>();
//         var prevCurve = fromCurve;
//         ICurve firstCurve = null;
//         while (true) {
//             visitedCurves.Add(prevCurve);
//             var curve = NextCurve(prevCurve, RotationDirection.Clockwise);
//             if(_placedCurves.Contains(curve))
//                 break;
//             if (visitedCurves.Contains(curve)) {
//                 firstCurve = curve;
//                 break;
//             }
//             prevCurve = curve;
//         }
//
//         if (firstCurve != null) {
//             var c = new Contour();
//             c.Curves.AddRange(visitedCurves.Skip(visitedCurves.IndexOf(firstCurve)));
//             _placedCurves.AddRange(c.Curves);
//             return c;
//         }
//         return null;
//     }
//
//     public Shape Walk() {
//
//         var outerContour = WalkFrom(FindOuterStartCurve());
//         var contours = Contour1.Curves
//             .Union(Contour2.Curves)
//             .Select(WalkFrom)
//             .Where(c => c != null)
//             .ToList();
//
//         var result = new Shape(outerContour);
//         result.InnerContours.AddRange(contours.Where(c => {
//             var rd = Contours.GetRotationDirection(c);
//             return rd == _rotationDirection.Invert();
//         }));
//
//         return result;
//     }
// }