using Juke.Querying;

namespace Juke.Ado.Net.Sql;

public class NameTarget {
    public required string FieldName { get; init; }
    public required string? DbFieldName { get; init; }
    public required Source Source { get; init; }
}