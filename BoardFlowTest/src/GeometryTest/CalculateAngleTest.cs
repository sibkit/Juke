using System;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Handling;
using Xunit;

namespace BoardFlowTest.GeometryTest;

public class CalculateAngleTest {

    void AssertWithAccuracy(double val1, double val2) {
        Assert.True(Math.Abs(val1-val2)<Geometry.Accuracy);
    }

    [Fact]
    public void TestCalculateAngle() {
        
        var sp = new Point(10, 10);
        var ep = new Point(20, 20);
        var cp = new Point(20, 10);
        var a1 = Angles.CalculateAngle(sp, ep, cp);
        AssertWithAccuracy(Angles.NegativeNormalize(a1), -Math.PI/2);

        var a2 = Angles.CalculateAngle(
            new Point(10, 20),
            new Point(10, 0),
            new Point(10, 10)
        );
        AssertWithAccuracy(Angles.PositiveNormalize(a2), Math.PI);
        
        var a3 = Angles.CalculateAngle(
            new Point(10, 10),
            new Point(10, -10),
            new Point(0, 0)
        );
        AssertWithAccuracy(a3, -Math.PI/2);
        
        var a4 = Angles.CalculateAngle(
            new Point(10, -10),
            new Point(10, 10),
            new Point(0, 0)
        );
        AssertWithAccuracy(Angles.PositiveNormalize(a4), Math.PI/2);
        
        var a5 = Angles.CalculateAngle(
            new Point(10, -10),
            new Point(10, 10),
            new Point(0, 0)
        );
        AssertWithAccuracy(Angles.PositiveNormalize(a5), Math.PI/2);
        
    }
}