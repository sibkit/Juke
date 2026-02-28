using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling;
using BoardFlow.Formats.Sgm.Handling.Relations;
using Xunit;

namespace BoardFlowTest.GeometryTest.RelationsTest;

public class RelationsTests {
    [Fact]
    public void TestLinesOverlapping() {
        var p1 = new Painter<Contour>(10, 10);
        p1.LineToInc(0,20);
        p1.LineToInc(10,0);
        p1.LineToInc(0,-20);
        p1.LineToInc(-10,0);
        var c1 = p1.Root;
        
         var p2 = new Painter<Contour>(20, 20);
         p2.LineToInc(0,10);
         p2.LineToInc(10,0);
         p2.LineToInc(0,-10);
         p2.LineToInc(-10,0);
         var c2 = p2.Root;

         var rel1 = RelationManager.DefineRelation(c1.Curves[2], c2.Curves[0]);
         Assert.True(rel1 is OverlappingRelation);
         Assert.Equal(2, ((rel1 as OverlappingRelation)!).Points.Count);
         Assert.True((rel1 as OverlappingRelation)!.Points[0].Point == new Point(20,30));
         Assert.True((rel1 as OverlappingRelation)!.Points[1].Point == new Point(20,20));
    }

    [Fact]
    public void TestArcsOverlapping() {
        var arc11 = new Arc {
            PointFrom = new Point(10, 10),
            PointTo = new Point(30, 10),
            Radius = 10,
            IsLargeArc = false,
            RotationDirection = RotationDirection.Clockwise
        };

        var arc12 = new Arc {
            PointFrom = new Point(20, 20),
            PointTo = new Point(20, 0),
            Radius = 10,
            IsLargeArc = false,
            RotationDirection = RotationDirection.Clockwise
        };
        
        var rel1 = RelationManager.DefineRelation(arc11, arc12);
        Assert.True(rel1 is OverlappingRelation);
        Assert.Equal(2, ((rel1 as OverlappingRelation)!).Points.Count);
        Assert.True((rel1 as OverlappingRelation)!.Points[0].Point == new Point(20,20));
        Assert.True((rel1 as OverlappingRelation)!.Points[1].Point == new Point(30,10));
    }

}