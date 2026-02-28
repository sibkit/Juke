namespace BoardFlow.Formats.Gerber.Entities.Apertures;

public class RectangleAperture: IAperture {
    public double XSize { get; set; }
    public double YSize { get; set; }
    public double? HoleDiameter { get; set; }
}