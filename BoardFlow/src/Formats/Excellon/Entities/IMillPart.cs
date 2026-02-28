using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Excellon.Entities;

public interface IMillPart
{
    public Point EndPoint { get; }
    public MillPartType PartType { get; }
}

public enum MillPartType
{
    Arc,
    Linear
}