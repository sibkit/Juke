using System.Collections.Generic;
using BoardFlow.Formats.Sgm.Entities.GraphicElements.Curves;

namespace BoardFlow.Board.Layers;

public class ScribingLayer: IBoardLayer {
    public required string Name { get; set; }
    public List<Line> Lines { get; } = [];
}