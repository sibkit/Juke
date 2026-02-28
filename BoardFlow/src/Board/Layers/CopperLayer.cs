using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Board.Layers;

public class CopperLayer: IStackLayer {
    public required string Name { get; set; }
    public required SgmDocument Image { get; init; }

    public required double BaseThickness { get; init; }
    public double? PlatingThickness { get; init; }

    public double Thickness => BaseThickness + PlatingThickness??0;
    
    
}