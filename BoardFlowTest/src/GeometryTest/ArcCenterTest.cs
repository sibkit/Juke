using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling;
using Xunit;

namespace BoardFlowTest.GeometryTest;

public class ArcCenterTest {
    [Fact]
    public void TestArcCenter() {
        var arc = new Arc {
            PointFrom = new Point(0, 0),
            PointTo = new Point(5, 5),
            IsLargeArc = true,
            Radius = 5,
            RotationDirection = RotationDirection.Clockwise
        };
        var cp = Geometry.ArcCenter(arc);
        Assert.True(cp == new Point(0,5));
        
        var arc2 = new Arc {
            PointFrom = new Point(10, -100),
            PointTo = new Point(20, -110),
            RotationDirection = RotationDirection.CounterClockwise,
            IsLargeArc = true,
            Radius = 10
        };
        var cp2 = Geometry.ArcCenter(arc2);
        Assert.True(cp2 == new Point(10,-110));
        
        
        var arc3 = new Arc {
            PointFrom = new Point(10, 100),
            PointTo = new Point(20, 110),
            RotationDirection = RotationDirection.Clockwise,
            IsLargeArc = false,
            Radius = 10
        };
        var cp3 = Geometry.ArcCenter(arc3);
        Assert.True(cp3 == new Point(20,100));
    }
}