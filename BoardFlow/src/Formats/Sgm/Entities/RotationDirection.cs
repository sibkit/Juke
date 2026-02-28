using System;

namespace BoardFlow.Formats.Sgm.Entities;

public enum RotationDirection {
    Clockwise,
    CounterClockwise,
    None
}

public static class RotationDirectionExtension {
    public static RotationDirection Invert(this RotationDirection direction) {
        //return direction;
        return direction switch {
            RotationDirection.None => RotationDirection.None,
            RotationDirection.Clockwise => RotationDirection.CounterClockwise,
            RotationDirection.CounterClockwise => RotationDirection.Clockwise,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}