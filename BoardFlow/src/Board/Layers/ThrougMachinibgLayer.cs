using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Board.Layers;

public class DrillLayer: IBoardLayer {
    public required string Name { get; set; }
    public SgmDocument Image { get; }
    public bool IsMetallization { get; }
    public IStackLayer? FromLayer { get; set; }
    public IStackLayer? ToLayer { get; set; }
    public DrillLayer(SgmDocument image, bool isMetallization) {
        Image = image;
        IsMetallization = isMetallization;
    }
}