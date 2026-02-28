using BoardFlow.Formats.Sgm.Entities;
using BoardFlow.Formats.Sgm.Handling;
using Xunit;

namespace BoardFlowTest.GeometryTest;

public class QuadrantsTest {
    [Fact]
    public void TestQuadrantTransitions() {
        var q1 = Quadrant.I;
        //var q2 = Quadrant.II;
        var q3 = Quadrant.III;
        var q4 = Quadrant.IV;

        Assert.Equal(QuadrantTransition.IV_I, Quadrants.GetTransitions(q4, q1, RotationDirection.CounterClockwise));
        Assert.Equal(QuadrantTransition.IV_I, Quadrants.GetTransitions(q1, q4, RotationDirection.Clockwise));
        Assert.Equal(QuadrantTransition.IV_I|QuadrantTransition.III_IV, Quadrants.GetTransitions(q1, q3, RotationDirection.Clockwise));
        Assert.Equal(QuadrantTransition.I_II|QuadrantTransition.II_III, Quadrants.GetTransitions(q1, q3, RotationDirection.CounterClockwise));
    }
}