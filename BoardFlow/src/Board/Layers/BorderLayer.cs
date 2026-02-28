using BoardFlow.Formats.Sgm.Entities.GraphicElements;

namespace BoardFlow.Board.Layers;

public class BorderLayer: IBoardLayer {
    public required string Name { get; set; }
    public required Shape Shape { get;set; }
    
}