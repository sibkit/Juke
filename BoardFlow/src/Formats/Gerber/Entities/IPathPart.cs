using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Gerber.Entities;

public interface IPathPart {
    public Point EndPoint { get; }
}