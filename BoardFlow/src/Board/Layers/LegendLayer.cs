using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Board.Layers;

public class LegendLayer {
    public required string Name { get; set; }
    public required SgmDocument Image { get; init; }
}