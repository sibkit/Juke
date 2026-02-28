using System.Collections.Generic;
using System.Linq;

namespace BoardFlow.Formats.Sgm.Entities.GraphicElements;

public class Shape: IGraphicElement {

    public List<Contour> OuterContours { get; } = [];
    public List<Contour> InnerContours { get; } = [];
    
    public Shape() { }
    
    public Shape(Contour contour) {
        OuterContours.Add(contour);
    }


    public Bounds Bounds {
        get {
            var bounds = Bounds.Empty();
            return OuterContours.Aggregate(bounds, (current, contour) => current.ExtendBounds(contour.Bounds));
        }
    }

    // public void NormalizeRotation() {
    //     foreach (var contour in OuterContours) {
    //         var rd = Contours.GetRotationDirection(contour);
    //         if (rd == RotationDirection.CounterClockwise) {
    //             
    //         }
    //     }
    // }
    
    // public void UpdateBounds() {

    //     foreach(var ic in InnerContours)
    //         ic.UpdateBounds();
    //     foreach(var ic in OuterContours)
    //         ic.UpdateBounds();
    // }

    public void Move(double dx, double dy) {
        foreach (var ic in InnerContours) {
            ic.Move(dx, dy);
        }

        foreach (var oc in OuterContours) {
            oc.Move(dx, dy);
        }
        // UpdateBounds();
    }
}