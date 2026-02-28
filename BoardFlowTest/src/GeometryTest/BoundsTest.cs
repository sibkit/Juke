using System;
using System.Runtime.Intrinsics;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling;
using Xunit;

namespace BoardFlowTest.GeometryTest;

public class BoundsTest {
    [Fact]
    public void TestBoundsIntersections() {
        var b1 = new Bounds(0,0,5,5);
        
        Assert.False(b1.IsIntersected(new Bounds(6,6,7,7)));
        Assert.True(b1.IsIntersected(new Bounds(-1,-1,7,7)));
        Assert.True(b1.IsIntersected(new Bounds(-1,-1,1,1)));
    }
    
    [Fact]
    public void TestContainsPoint() {
        var b1 = new Bounds(0,0,5,5);
        
        Assert.True(b1.Contains(new Point(0,0)));
        Assert.True(b1.Contains(new Point(5,5)));
        Assert.True(b1.Contains(new Point(2,3)));
        
        Assert.False(b1.Contains(new Point(-1,0)));
        Assert.False(b1.Contains(new Point(0,-1)));
        Assert.False(b1.Contains(new Point(6,5)));
        Assert.False(b1.Contains(new Point(2,5.0001)));
        Assert.False(b1.Contains(new Point(-0.000001,2)));
    }

    [Fact]
    public void TestArcBounds() {
        var arc1 = new Arc {
            PointFrom = new Point(0, 0),
            PointTo = new Point(5, 5),
            RotationDirection = RotationDirection.Clockwise,
            IsLargeArc = false,
            Radius = 5
        };

        var b1 = arc1.Bounds;
        Assert.True(Math.Abs(b1.GetWidth() - 5) < Geometry.Accuracy);
        Assert.True(Math.Abs(b1.GetHeight() - 5) < Geometry.Accuracy);
        Assert.True(b1.MinPoint == new Point(0, 0));
        Assert.True(b1.MaxPoint == new Point(5, 5));
        
        arc1.IsLargeArc = true;
        var b2 = arc1.Bounds;
        Assert.True(Math.Abs(b2.GetWidth() - 10) < Geometry.Accuracy);
        Assert.True(Math.Abs(b2.GetHeight() - 10) < Geometry.Accuracy);
        Assert.True(b2.MinPoint == new Point(-5, 0));
        Assert.True(b2.MaxPoint == new Point(5, 10));

        var arc2 = new Arc {
            PointFrom = new Point(0, 0),
            PointTo = new Point(-5, -5),
            RotationDirection = RotationDirection.CounterClockwise,
            IsLargeArc = true,
            Radius = 5
        };
        b2 = arc2.Bounds;
        Assert.True(b2.MinPoint == new Point(-10, -5));
        Assert.True(b2.MaxPoint == new Point(0, 5));

        var arc3 = new Arc {
            PointFrom = new Point(0, 0),
            PointTo = new Point(-5, -5),
            RotationDirection = RotationDirection.Clockwise,
            IsLargeArc = true,
            Radius = 5
        };
        
        var b3 = arc3.Bounds;
        Assert.True(b3.MinPoint == new Point(-5, -10));
        Assert.True(b3.MaxPoint == new Point(5, 0));

        var arc4 = new Arc {
            PointFrom = new Point(10, -100),
            PointTo = new Point(20, -110),
            RotationDirection = RotationDirection.CounterClockwise,
            IsLargeArc = true,
            Radius = 10
        };
        var b4 = arc4.Bounds;
        Assert.True(b4.MinPoint == new Point(0, -120));
        Assert.True(b4.MaxPoint == new Point(20, -100));
        
        var arc5 = new Arc {
            PointFrom = new Point(70, -110),
            PointTo = new Point(80, -100),
            RotationDirection = RotationDirection.CounterClockwise,
            IsLargeArc = true,
            Radius = 10
        };
        var b5 = arc5.Bounds;
        Assert.True(b5.MinPoint == new Point(70, -110));
        Assert.True(b5.MaxPoint == new Point(80, -100));
    }


}