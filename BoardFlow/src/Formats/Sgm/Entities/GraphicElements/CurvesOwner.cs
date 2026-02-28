using System.Collections.Generic;
using System.Linq;

namespace BoardFlow.Formats.Sgm.Entities.GraphicElements;

public abstract class CurvesOwner: IGraphicElement {
    

    public List<ICurve> Curves { get; } = [];

    public void Reverse() {
        Curves.Reverse();
        foreach (var curve in Curves) {
            curve.Reverse();
        }
    }
    
    public void Move(double dx, double dy) {
        foreach (var curve in Curves) {
            curve.Move(dx, dy);
        }
    }

    
    
    public Bounds Bounds {
        get {
            var bounds = Bounds.Empty();
            return Curves.Aggregate(bounds, (current, p) => current?.ExtendBounds(p.Bounds) ?? p.Bounds);
        }
    }


}