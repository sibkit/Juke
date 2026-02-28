using System;
using System.Collections.Generic;
using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Entities.GraphicElements;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;
using BoardFlow.Formats.Sgm.Handling;
using BoardFlow.Formats.Svg.Entities;
using BoardFlow.Formats.Svg.Writing;
using Xunit;
using SgmPath = BoardFlow.Formats.Sgm.Entities.GraphicElements.Path;

namespace BoardFlowTest.GeometryTest;

public class SgmElementsTest {
    [Fact]
    public void ContourCreationFromPainter_UsesExpectedCoordinates() {
        var painter = new Painter<Contour>(0, 0);
        painter.LineToInc(10, 0);
        painter.LineToInc(0, 5);
        painter.ArcToInc(-10, 0, 5, RotationDirection.Clockwise, false);
        painter.LineToAbs(0, 0);

        var contour = painter.Root;

        Assert.Equal(4, contour.Curves.Count);

        var line1 = Assert.IsType<Line>(contour.Curves[0]);
        Assert.True(line1.PointFrom == new Point(0, 0));
        Assert.True(line1.PointTo == new Point(10, 0));

        var line2 = Assert.IsType<Line>(contour.Curves[1]);
        Assert.True(line2.PointFrom == new Point(10, 0));
        Assert.True(line2.PointTo == new Point(10, 5));

        var arc = Assert.IsType<Arc>(contour.Curves[2]);
        Assert.True(arc.PointFrom == new Point(10, 5));
        Assert.True(arc.PointTo == new Point(0, 5));

        var line3 = Assert.IsType<Line>(contour.Curves[3]);
        Assert.True(line3.PointFrom == new Point(0, 5));
        Assert.True(line3.PointTo == new Point(0, 0));
    }

    [Fact]
    public void PathMove_ShiftsCurveCoordinates() {
        var painter = new Painter<SgmPath>(5, 5);
        painter.LineToInc(5, 0);
        painter.LineToInc(0, 5);

        var path = painter.Root;
        path.Move(-2, 3);

        var line1 = Assert.IsType<Line>(path.Curves[0]);
        Assert.True(line1.PointFrom == new Point(3, 8));
        Assert.True(line1.PointTo == new Point(8, 8));

        var line2 = Assert.IsType<Line>(path.Curves[1]);
        Assert.True(line2.PointFrom == new Point(8, 8));
        Assert.True(line2.PointTo == new Point(8, 13));
    }

    [Fact]
    public void ShapeBounds_FromOuterContour() {
        var painter = new Painter<Contour>(-5, -5);
        painter.LineToInc(10, 0);
        painter.LineToInc(0, 10);
        painter.LineToInc(-10, 0);
        painter.LineToInc(0, -10);

        var shape = new Shape(painter.Root);
        var bounds = shape.Bounds;

        Assert.True(bounds.MinPoint == new Point(-5, -5));
        Assert.True(bounds.MaxPoint == new Point(5, 5));
    }

    [Fact]
    public void DotMove_UpdatesCoordinatesAndBounds() {
        var dot = new Dot {
            CenterPoint = new Point(2, 3),
            Diameter = 4
        };

        Assert.True(dot.Bounds.MinPoint == new Point(0, 1));
        Assert.True(dot.Bounds.MaxPoint == new Point(4, 5));

        dot.Move(-1, 2);

        Assert.True(dot.CenterPoint == new Point(1, 5));
        Assert.True(dot.Bounds.MinPoint == new Point(-1, 3));
        Assert.True(dot.Bounds.MaxPoint == new Point(3, 7));
    }

    [Fact]
    public void StarInCircle_Creation() {
        double radius = 50;
        double epsilon = 1e-5;

        // Circle using 4 arcs
        var circlePainter = new Painter<Contour>(radius, 0);
        circlePainter.ArcToInc(-radius, radius, radius, RotationDirection.CounterClockwise, false);
        circlePainter.ArcToInc(-radius, -radius, radius, RotationDirection.CounterClockwise, false);
        circlePainter.ArcToInc(radius, -radius, radius, RotationDirection.CounterClockwise, false);
        circlePainter.ArcToInc(radius, radius, radius, RotationDirection.CounterClockwise, false);
        var circle = circlePainter.Root;

        Assert.Equal(4, circle.Curves.Count);

        // Star (as a non-self-intersecting decagon)
        double outerRadius = radius;
        double innerRadius = outerRadius * (3.0 - Math.Sqrt(5.0)) / 2.0; 

        var starPoints = new List<Point>();
        for (int i = 0; i < 5; i++) {
            double outerAngleRad = (90 + i * 72) * Math.PI / 180.0;
            starPoints.Add(new Point(
                Math.Round(outerRadius * Math.Cos(outerAngleRad), 6),
                Math.Round(outerRadius * Math.Sin(outerAngleRad), 6)
            ));
            double innerAngleRad = (90 + 36 + i * 72) * Math.PI / 180.0;
            starPoints.Add(new Point(
                Math.Round(innerRadius * Math.Cos(innerAngleRad), 6),
                Math.Round(innerRadius * Math.Sin(innerAngleRad), 6)
            ));
        }

        var starPainter = new Painter<Contour>(starPoints[0].X, starPoints[0].Y);
        for (int i = 1; i < starPoints.Count; i++) {
            starPainter.LineToAbs(starPoints[i].X, starPoints[i].Y);
        }
        starPainter.LineToAbs(starPoints[0].X, starPoints[0].Y);
        
        var star = starPainter.Root;
        Assert.Equal(10, star.Curves.Count);

        var circleBounds = new Shape(circle).Bounds;
        var starBounds = new Shape(star).Bounds;

        Assert.True(starBounds.MinPoint.X >= circleBounds.MinPoint.X - epsilon);
        Assert.True(starBounds.MinPoint.Y >= circleBounds.MinPoint.Y - epsilon);
        Assert.True(starBounds.MaxPoint.X <= circleBounds.MaxPoint.X + epsilon);
        Assert.True(starBounds.MaxPoint.Y <= circleBounds.MaxPoint.Y + epsilon);
        
        var shape = new Shape(circle);
        shape.InnerContours.Add(star);
        
        var svgDoc = new SvgDocument();
        svgDoc.Elements.Add(shape);
        
        SvgWriter.Write(svgDoc, "star_in_circle.svg");
    }
}
