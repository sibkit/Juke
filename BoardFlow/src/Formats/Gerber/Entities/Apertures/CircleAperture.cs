namespace BoardFlow.Formats.Gerber.Entities.Apertures;

public class CircleAperture : IAperture {
    public double Diameter { get; init; }
    public double? HoleDiameter { get; init; }
}