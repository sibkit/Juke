using System;
using System.Text.RegularExpressions;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling;
using Xunit;

namespace BoardFlowTest.GeometryTest;

public class VectorsTest {
    [Fact]
    public void TestVectorsAngle() {
        Assert.Equal(-Math.PI/2d, Vectors.Angle(new Vector(0,10), new Vector(10,0)));
    }

    [Fact]
    public void TestStartCurveVector() {
        var cos45 = Math.Cos(Math.PI / 4d);

        
        var arc1 = new Arc {
            PointFrom = new Point(2d + cos45, 2d + cos45),
            PointTo = new Point(2d - cos45, 2d + cos45),
            RotationDirection = RotationDirection.CounterClockwise,
            Radius = 1d,
            IsLargeArc = false
        };
        
        var arc2 = new Arc {
            PointFrom = new Point(2d - cos45, 2d + cos45),
            PointTo = new Point(2d + cos45, 2d + cos45),
            RotationDirection = RotationDirection.Clockwise,
            Radius = 1d,
            IsLargeArc = false
        };
        
        var arc3 = new Arc {
            PointFrom = new Point(2d, 3d),
            PointTo = new Point(1d, 2d),
            RotationDirection = RotationDirection.CounterClockwise,
            Radius = 1d,
            IsLargeArc = false
        };
        
        var arc4 = new Arc {
            PointFrom = new Point(2d, 1d),
            PointTo = new Point(2d , 3d),
            RotationDirection = RotationDirection.Clockwise,
            Radius = 1d,
            IsLargeArc = true
        };
        
        var arc5= new Arc {
            PointFrom = new Point(2d, 1d),
            PointTo = new Point(1d, 2d),
            RotationDirection = RotationDirection.CounterClockwise,
            Radius = 1d,
            IsLargeArc = true
        };
        
        var arc6 = new Arc {
            PointFrom = new Point(2d - cos45, 2d - cos45),
            PointTo = new Point(2d + cos45, 2d - cos45),
            RotationDirection = RotationDirection.CounterClockwise,
            Radius = 1d,
            IsLargeArc = false
        };



        var vOut1 = Curves.GetTangentOutVector(arc1);
        var vOut2 = Curves.GetTangentOutVector(arc2);
        var vOut3 = Curves.GetTangentOutVector(arc3);
        var vOut4 = Curves.GetTangentOutVector(arc4);
        var vOut5 = Curves.GetTangentOutVector(arc5);
        var vOut6 = Curves.GetTangentOutVector(arc6);
        
        var vIn1 = Curves.GetTangentInVector(arc1);
        var vIn2 = Curves.GetTangentInVector(arc2);
        var vIn3 = Curves.GetTangentInVector(arc3);
        var vIn4 = Curves.GetTangentInVector(arc4);
        var vIn5 = Curves.GetTangentInVector(arc5);
        var vIn6 = Curves.GetTangentInVector(arc6);
        

    }
}