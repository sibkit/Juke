using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Gerber.Entities;

public class FlashOperation: IPaintOperation {
    public Point Point{get;init;}
    public int ApertureCode{get;init;}
}