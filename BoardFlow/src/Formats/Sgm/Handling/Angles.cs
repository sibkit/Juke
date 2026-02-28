using System;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Sgm.Handling;

public static class Angles {
    public static double CalculateAngle(Point sp, Point ep, Point cp) {
        return  Math.Atan2(ep.Y - cp.Y, ep.X - cp.X) - Math.Atan2(sp.Y - cp.Y, sp.X - cp.X);
    }

    public static double PiNormalize(double angle) {
        return angle switch {
            < -Math.PI => PiNormalize(angle + 2*Math.PI),
            > Math.PI => PiNormalize(angle - 2*Math.PI),
            _ => angle
        };
    }
    
    public static double PositiveNormalize(double angle) {
        return angle switch {
            <= 0 => PositiveNormalize(angle + 2*Math.PI),
            > Math.PI*2d => PositiveNormalize(angle - 2*Math.PI),
            _ => angle
        };
    }
    
    public static double NegativeNormalize(double angle) {
        return angle switch {
            >= 0 => NegativeNormalize(angle - 2*Math.PI),
            < -Math.PI*2d => NegativeNormalize(angle + 2*Math.PI),
            _ => angle
        };
    }
}