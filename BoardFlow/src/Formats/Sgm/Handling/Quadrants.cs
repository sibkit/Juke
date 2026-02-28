using System;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Sgm.Handling;

public static class Quadrants {
    public static Quadrant GetQuadrant(double prX, double prY) {
        
        if(Math.Abs(prX)<Geometry.Accuracy)
            return prY >= 0 ? Quadrant.I_II : Quadrant.III_IV;
        
        if(Math.Abs(prY)<Geometry.Accuracy)
            return prX >= 0 ? Quadrant.IV_I : Quadrant.II_III;
        
        if (prX > 0) {
            return prY >= 0 ? Quadrant.I : Quadrant.IV;
        }
        return prY >= 0 ? Quadrant.II : Quadrant.III;
    }

    public static QuadrantTransition GetTransitions(Quadrant startQuadrant, Quadrant endQuadrant, RotationDirection rd) {
        var result = QuadrantTransition.None;
        switch (rd) {
            case RotationDirection.Clockwise:
                var cQ = startQuadrant;
                while (cQ != endQuadrant) {
                    cQ = cQ.Prev();
                    switch (cQ) {
                        case Quadrant.I:
                            result |= QuadrantTransition.I_II;
                            break;
                        case Quadrant.II:
                            result |= QuadrantTransition.II_III;
                            break;
                        case Quadrant.III:
                            result |= QuadrantTransition.III_IV;
                            break;
                        case Quadrant.IV:
                            result |= QuadrantTransition.IV_I;
                            break;
                        default:
                            throw new Exception("Unknown Quadrant: " + cQ);
                    }
                }
                break;
            case RotationDirection.CounterClockwise:
                cQ = startQuadrant;
                while (cQ != endQuadrant) {
                    cQ = cQ.Next();
                    switch (cQ) {
                        case Quadrant.I:
                            result |= QuadrantTransition.IV_I;
                            break;
                        case Quadrant.II:
                            result |= QuadrantTransition.I_II;
                            break;
                        case Quadrant.III:
                            result |= QuadrantTransition.II_III;
                            break;
                        case Quadrant.IV:
                            result |= QuadrantTransition.III_IV;
                            break;
                        default:
                            throw new Exception("Unknown Quadrant: " + cQ);
                    }
                    
                }
                break;
            default:
                throw new Exception("Unknown rotation direction: " + rd);
        }
        return result;
    }
}

[Flags]
public enum Quadrant {
    None = 0,
    I = 1,
    II = 2,
    III = 4,
    IV = 8,
    I_II = I | II,
    II_III = II | III,
    III_IV = III | IV,
    IV_I = IV |I
}

public static class QuadrantExtensions
{
    public static Quadrant Next(this Quadrant quadrant) {
        return quadrant switch {
            Quadrant.I => Quadrant.II,
            Quadrant.II => Quadrant.III,
            Quadrant.III => Quadrant.IV,
            Quadrant.IV => Quadrant.I,
            _ => throw new ArgumentOutOfRangeException(nameof(quadrant), quadrant, null)
        };
    }
    
    public static Quadrant Prev(this Quadrant quadrant) {
        return quadrant switch {
            Quadrant.I => Quadrant.IV,
            Quadrant.IV => Quadrant.III,
            Quadrant.III => Quadrant.II,
            Quadrant.II => Quadrant.I,
            _ => throw new ArgumentOutOfRangeException(nameof(quadrant), quadrant, null)
        };
    }
}

[Flags]
public enum QuadrantTransition {
    None = 0,
    I_II = 1,
    II_III = 2,
    III_IV = 4,
    IV_I = 8
}