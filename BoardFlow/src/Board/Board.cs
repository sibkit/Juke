using System.Collections.Generic;
using System.Drawing;

namespace BoardFlow.Board;

public interface IBoardLayer {
    string Name { get; }
}

public interface IStackLayer : IBoardLayer {
    double Thickness { get; }
}

public class BoardBox {
    public Point ShiftPoint { get; set; }
    public double Angle { get; set; }
}

public class Board {
    public List<BoardBox> ChildBoards { get; } = [];
    public List<IBoardLayer> Layers { get; } = [];
}

