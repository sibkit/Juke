using System;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Sgm.Handling;

public readonly struct Vector(double x, double y) {

    public Vector(Point p) : this(p.X, p.Y) { }
    
    public double X { get; } = x;
    public double Y { get; } = y;

    public override string ToString() {
        return $"(x:{X},y: {Y})";
    }
}

public static class Vectors {
    // public static double DotProduct(Vector a, Vector b) {
    //     return a.X * b.X + a.Y * b.Y;
    // }

    /// <summary>
    /// Векторное произведение векторов
    /// </summary>
    /// <param name="a">Вектор a</param>
    /// <param name="b">Вектор b</param>
    /// <returns>Значение векторного произведения</returns>
    public static double CrossProduct(Vector a, Vector b) {
        return a.X * b.Y - a.Y * b.X;
    }

    public static double Length(Vector v) {
        return Math.Sqrt(v.X * v.X + v.Y * v.Y);
    }

    public static double Angle(Vector a, Vector b) {
        return Angles.PiNormalize(Math.Atan2(b.Y, b.X) - Math.Atan2(a.Y, a.X));
        
    }
}