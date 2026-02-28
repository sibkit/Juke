namespace BoardFlow.Board.Layers;

public enum MaterialType {
    FR4,
    Aluminium
}

public class MaterialLayer: IStackLayer {
    public required string Name { get; set; }
    public MaterialLayer(MaterialType materialType, double thickness) {
        MaterialType = materialType;
        Thickness = thickness;
    }
    public MaterialType MaterialType { get; }
    public double Thickness { get; }
}