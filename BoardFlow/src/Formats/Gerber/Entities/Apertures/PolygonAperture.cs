namespace BoardFlow.Formats.Gerber.Entities.Apertures;

public class PolygonAperture: IAperture {
    public double OuterDiameter { get; set; }
    public int VerticesCount { get; set; }
    public double? Rotation { get; set; }
    public double? HoleDiameter { get; set; }
}