using System.Collections.Generic;
using BoardFlow.Formats.Common.Entities;
using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Svg.Entities;

public class SvgDocument {
    public Uom? Uom { get; set; }
    public Bounds? ViewBox { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public List<IGraphicElement> Elements { get; } = [];
}