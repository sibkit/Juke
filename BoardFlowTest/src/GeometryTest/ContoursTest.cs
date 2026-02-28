using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using BoardFlow.Converters.SgmToSvg;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Handling;
using BoardFlow.Formats.Svg.Writing;
using Xunit;

namespace BoardFlowTest.GeometryTest;

public class ContoursTest {
    [Fact]
    public void ContourSplitTest() {
        var p1 = new Painter<Contour>(10, 10);
        p1.LineToInc(0, 60);
        p1.LineToInc(40, 0);
        p1.LineToInc(0, -60);
        p1.LineToInc(-40, 0);
        var c1 = p1.Root;

        var p2 = new Painter<Contour>(40, 30);
        p2.LineToInc(20, 20);
        p2.LineToInc(-30, 30);
        p2.LineToInc(50, 0);
        p2.LineToInc(0, -50);
        p2.LineToInc(-40, 0);
        var c2 = p2.Root;

        var cs1 = Contours.SplitByRelationPoints(c1, c2);
        Assert.True(cs1.Curves[1].PointTo == new Point(40, 70));
        Assert.True(cs1.Curves[2].PointTo == new Point(50, 70));
        Assert.True(cs1.Curves[3].PointTo == new Point(50, 60));
        Assert.True(cs1.Curves[4].PointTo == new Point(50, 40));
        Assert.True(cs1.Curves[5].PointTo == new Point(50, 30));

        var cs2 = Contours.SplitByRelationPoints(c2, c1);
        Assert.True(cs2.Curves[0].PointTo == new Point(50, 40));
        Assert.True(cs2.Curves[7].PointTo == new Point(50, 30));
    }


    [Fact]
    public void RoundPointTest() {
        var r = new Random();
        var x1 = r.NextDouble();
        var x2 = x1 + 0.000_000_000_001;

        var y1 = r.NextDouble();
        var y2 = y1 + 0.000_000_000_001;

        var p1 = Geometry.RoundPoint(new Point(x1, y1));
        var p2 = Geometry.RoundPoint(new Point(x2, y2));
        var p3 = Geometry.RoundPoint(new Point(x1 + 0.000_000_000_1, y2 + 0.000_000_000_1));
        Assert.Equal(p1, p2);
        Assert.NotEqual(p1, p3);
    }

    [Fact]
    public void TestPointsMap() {
        // var p1 = new Painter<Contour>(10, 10);
        // p1.LineToInc(0, 60);
        // p1.LineToInc(40, 0);
        // p1.LineToInc(0, -60);
        // p1.LineToInc(-40, 0);
        // var c1 = p1.Root;
        //
        // var p2 = new Painter<Contour>(40, 30);
        // p2.LineToInc(20, 20);
        // p2.LineToInc(-30, 30);
        // p2.LineToInc(50,0);
        // p2.LineToInc(0,-50);
        // p2.LineToInc(-40,0);
        // var c2 = p2.Root;
        //
        // var map = ContoursHandler.GetPointsMap(c1, c2);
        // Assert.Equal(13, map.Count);
    }

    [Fact]
    public void TestContoursWalker() {
        var p1 = new Painter<Contour>(10, 10);
        p1.LineToInc(0, 60);
        p1.LineToInc(40, 0);
        p1.LineToInc(0, -60);
        p1.LineToInc(-40, 0);
        var c1 = p1.Root;

        var p2 = new Painter<Contour>(40, 30);
        p2.LineToInc(20, 20);
        p2.LineToInc(-30, 30);
        p2.LineToInc(50, 0);
        p2.LineToInc(0, -50);
        p2.LineToInc(-40, 0);
        var c2 = p2.Root;

        var ms = Contours.Union(c1, c2);
        Assert.Equal(8, ms.OuterContours[0].Curves.Count);
        Assert.Single(ms.InnerContours);
        // var mergedContour = new ContoursWalker(c1, c2).WalkMerge();
        // Assert.Equal(8, mergedContour.Curves.Count);
    }


    private List<Contour> GetContours() {
        var contours = new List<Contour>();

        var p1 = new Painter<Contour>(40, 50);
        p1.ArcToInc(20,0,10,RotationDirection.Clockwise, false);
        p1.LineToInc(0,-30);
        p1.ArcToInc(-10,-10,10,RotationDirection.Clockwise, false);
        p1.LineToInc(-30, 0);
        p1.LineToInc(0, 10);
        p1.ArcToInc(10,10,10,RotationDirection.Clockwise, false);
        p1.ArcToInc(10,10,10,RotationDirection.CounterClockwise, false);
        p1.LineToInc(0,10);
        
        contours.Add(p1.Root);

        var p2 = new Painter<Contour>(70, 50);
        p2.LineToInc(0,-30);
        p2.ArcToInc(-20,0,10,RotationDirection.Clockwise, false);
        p2.LineToInc(0,10);
        p2.ArcToInc(20,20,20,RotationDirection.Clockwise, false);
        
        contours.Add(p2.Root);

        var p3 = new Painter<Contour>(30, 20);
        p3.LineToInc(-10,0);
        p3.ArcToInc(0,20,10,RotationDirection.Clockwise, false);
        p3.LineToInc(20,0);
        p3.ArcToInc(10,-10,10,RotationDirection.Clockwise, false);
        p3.LineToInc(0,-10);
        p3.ArcToInc(-20,0,10,RotationDirection.Clockwise, false);
        
        contours.Add(p3.Root);

        return contours;
    }

    private List<Contour> GetContours2() {
        var contours = new List<Contour>();

        var p1 = new Painter<Contour>(20, 10);
        p1.ArcToInc(0, 40, 20.2, RotationDirection.CounterClockwise, false);
        p1.ArcToInc(0, -40, 20, RotationDirection.Clockwise, false);

        contours.Add(p1.Root);

        var p2 = new Painter<Contour>(50, 10);
        p2.ArcToInc(0, 40, 20, RotationDirection.Clockwise, false);
        p2.ArcToInc(0, -40, 20.2, RotationDirection.CounterClockwise, false);

        contours.Add(p2.Root);

        var p3 = new Painter<Contour>(30, 20);
        p3.LineToInc(-10,0);
        p3.ArcToInc(0,20,10,RotationDirection.Clockwise, false);
        p3.LineToInc(20,0);
        p3.ArcToInc(10,-10,10,RotationDirection.Clockwise, false);
        p3.LineToInc(0,-10);
        p3.ArcToInc(-20,0,10,RotationDirection.Clockwise, false);
        
        contours.Add(p3.Root);

        return contours;
    }


    [Fact]
    public void MoveTest() {
        var area = new SgmDocument();
        var cts = GetContours2();
        
        for (var i = 0; i < 5; i++) {
            for (var j = 0; j < 5; j++) {
                var cCloned = cts[0].Clone();
                cCloned.Move(j*80, i*80);
                area.GraphicElements.Add(cCloned);
            }
        }
        

        
        //var c1 = Contours.Union(cts[0], cts[1]);
        //c1 = Contours.Union(cts[0], c1.OuterContour);
        //area.GraphicElements.Add(c1);
        area.InvertYAxe();

        var layer = SgmToSvgConverter.Convert(area);
        SvgWriter.Write(layer,"D://5c.svg");
    }
    
    [Fact]
    public void TestContoursOperation() {

        var area = new SgmDocument();
        var cts = GetContours2().Union(GetContours()).ToList();

        var line = 0;
        var sw = 80d;
        for (var i = 0; i < cts.Count; i++) {
            var ci = cts.ElementAt(i);
            for (var j = i + 1; j < cts.Count; j++) {
                var cj = cts.ElementAt(j);

                
                var ciCloned = ci.Clone();
                if(Contours.GetRotationDirection(ciCloned) != RotationDirection.Clockwise)
                    ciCloned.Reverse();
                ciCloned.Move(0,line*sw);
                area.GraphicElements.Add(ciCloned);
                
                var cjCloned = cj.Clone();
                if(Contours.GetRotationDirection(cjCloned) != RotationDirection.Clockwise)
                    cjCloned.Reverse();
                cjCloned.Move(sw,line*sw);
                area.GraphicElements.Add(cjCloned);
                
                
                var uc =  Contours.Union(ci.Clone(), cj.Clone());
                
                if (uc != null) {
                    uc.Move(2*sw, line*sw);
                    area.GraphicElements.Add(uc);
                }
                
                var ss = Contours.Subtract(ci.Clone(), cj.Clone());
                if (ss != null) {
                    ss.Move(3*sw, line*sw);
                    area.GraphicElements.Add(ss);
                }
                line++;
            }
            
        }
        
        //var c1 = Contours.Union(cts[0], cts[1]);
        //c1 = Contours.Union(cts[0], c1.OuterContour);
        //area.GraphicElements.Add(c1);
        area.InvertYAxe();

        var layer = SgmToSvgConverter.Convert(area);
        SvgWriter.Write(layer,"D://4c.svg");
    }

    [Fact]
    public void TestSimpleUnion() {
        var p1 = new Painter<Contour>(10, 10);
        p1.LineToInc(0,20);
        p1.LineToInc(20,0);
        p1.LineToInc(0,-20);
        p1.LineToInc(-20,0);
        
        var p2 = new Painter<Contour>(20, 20);
        p2.LineToInc(0,20);
        p2.LineToInc(20,0);
        p2.LineToInc(0,-20);
        p2.LineToInc(-20,0);

        var shape = Contours.Union(p1.Root, p2.Root);
        
        var area = new SgmDocument();
        area.GraphicElements.Add(shape);
        var svg = SgmToSvgConverter.Convert(area);
        SvgWriter.Write(svg, "D://ts1.svg");
    }
}