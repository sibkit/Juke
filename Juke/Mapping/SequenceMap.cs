namespace Juke.Mapping;

public class SequenceMap {
    public required string SequenceName { get; init; }
    public required string DbSequenceName { get; init; }
    public required Type SequenceValueType { get; init; }
}