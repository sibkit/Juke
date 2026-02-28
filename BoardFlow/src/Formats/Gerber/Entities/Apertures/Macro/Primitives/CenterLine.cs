namespace BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Primitives;

public class CenterLine : IPrimitive {
    public IExpression Exposure { get; set; }
    public IExpression Width { get; set; }
    public IExpression Height { get; set; }
    public IExpression CenterX { get; set; }
    public IExpression CenterY { get; set; }
    public IExpression Rotation { get; set; }
}