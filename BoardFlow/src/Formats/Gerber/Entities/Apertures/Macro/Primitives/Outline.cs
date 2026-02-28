using System.Collections.Generic;

namespace BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Primitives;



public class Outline: IPrimitive {
    public IExpression Exposure { get; set; }
    public List<(IExpression, IExpression)> Vertices { get; } = [];
    public IExpression Rotation { get; set; }
}