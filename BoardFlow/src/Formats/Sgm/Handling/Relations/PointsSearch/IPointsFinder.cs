using System.Collections.Generic;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;

namespace BoardFlow.Formats.Sgm.Handling.Relations.PointsSearch;

public interface IPointsFinder<in TF, in TS> 
    where TF : ICurve
    where TS: ICurve {
    (List<Point> points, bool isMatch) FindContactPoints(TF curve1, TS curve2);
}