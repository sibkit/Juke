using System.Collections.Generic;
using BoardFlow.Formats.Common;
using BoardFlow.Formats.Common.Entities;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro;

namespace BoardFlow.Formats.Gerber.Entities;

public class GerberDocument
{
    public List<IPaintOperation> Operations { get; } = [];
    public Uom? Uom {get; set;} = null;
    public Dictionary<int,IAperture> Apertures { get; } = new();
    public Dictionary<string, MacroApertureTemplate> MacroApertureTemplates { get; } = new();
}

