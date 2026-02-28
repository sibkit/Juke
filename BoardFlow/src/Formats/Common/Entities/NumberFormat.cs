namespace BoardFlow.Formats.Common.Entities;

public enum Zeros
{
    Leading,
    Trailing,
    All
}

public class NumberFormat() {
    public NumberFormat(int? left, int? right) : this() {
        Left = left;
        Right = right;
    }
    public Zeros? Zeros { get; set; } = null;
    public int? Left{get;set;} = null;
    public int? Right{get;set;} = null;
}

