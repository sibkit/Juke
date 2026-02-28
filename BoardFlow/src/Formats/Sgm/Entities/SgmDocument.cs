using System.Collections.Generic;
using BoardFlow.Formats.Common.Entities;

namespace BoardFlow.Formats.Sgm.Entities;

public class SgmDocument {
    public Uom? Uom { get; set; }
    public List<IGraphicElement> GraphicElements { get; } = [];
    public Bounds? Bounds { get; set; }
}