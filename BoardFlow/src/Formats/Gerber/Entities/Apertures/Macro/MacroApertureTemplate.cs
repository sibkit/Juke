using System.Collections.Generic;

namespace BoardFlow.Formats.Gerber.Entities.Apertures.Macro;

public class MacroApertureTemplate {
    public MacroApertureTemplate(string templateName) {
        Name = templateName;
    }
    
    public string Name {get;}
    public List<IPrimitive> Primitives {get;} = [];
    public Dictionary<string, IExpression> Expressions {get;} = [];
    public List<string> Comments {get;} = [];
}