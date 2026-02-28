using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Board.Layers;

public enum SolderMaskColor {
    Any,
    White,
    Green,
    Black,
    MatteWhite,
    MatteBlack
}

public class SolderMaskLayer: IBoardLayer {
    public required string Name { get; set; }
    public SolderMaskLayer(SolderMaskColor color, SgmDocument image) {
        Color = color;
        Image = image;
    }
    public SolderMaskColor Color { get; }
    public SgmDocument Image { get; }
}