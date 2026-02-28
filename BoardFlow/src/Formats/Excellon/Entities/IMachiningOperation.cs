using BoardFlow.Formats.Sgm.Entities;

namespace BoardFlow.Formats.Excellon.Entities;

public interface IMachiningOperation
{
    public Point StartPoint { get; set; }

    public IMachiningOperation CloneWithShift(Point shift);
}

