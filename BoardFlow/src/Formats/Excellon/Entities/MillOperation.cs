using System.Collections.Generic;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Excellon.Entities;

public readonly struct ArcMillPart(Point endPoint, float radius) {
    public Point EndPoint { get; } = endPoint;
    public MillPartType PartType { get; } = MillPartType.Arc;
    public float Radius { get; } = radius;
}

public readonly struct LinearMillPart(Point endPoint) : IMillPart {
    public Point EndPoint { get; } = endPoint;
    public MillPartType PartType { get; } = MillPartType.Linear;
    
}

public class MillOperation: IMachiningOperation {
    public Point StartPoint { get; set; }
    public List<IMillPart> MillParts { get; } = [];
    public IMachiningOperation CloneWithShift(Point shift) {
        var result = new MillOperation {
            StartPoint = StartPoint + shift
        };
        return result;
    }
}