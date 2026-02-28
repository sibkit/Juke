namespace BoardFlow.Formats.Sgm.Entities;

public interface IGraphicElement {
    
    
    Bounds Bounds { get; }
    //void UpdateBounds();
    
    void Move(double dx, double dy);
    
} 