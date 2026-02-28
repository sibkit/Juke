namespace BoardFlow.Formats.Gerber.Entities.Apertures;

public class ObRoundAperture: IAperture {
    public double XSize { get; set; }
    public double YSize { get; set; }
    public double? HoleDiameter { get; set; }
}