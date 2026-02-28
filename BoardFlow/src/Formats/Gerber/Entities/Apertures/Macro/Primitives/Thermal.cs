namespace BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Primitives;

public class Thermal: IPrimitive{
    public IExpression CenterX { get; set; }
    public IExpression CenterY { get; set; }
    public IExpression OuterDiameter { get; set; }
    public IExpression InnerDiameter { get; set; }
    public IExpression GapThickness { get; set; }
    public IExpression Rotation { get; set; }
}