namespace Juke.Querying;

public enum SortOrderDirection {
    Asc, 
    Desc
}

public class SortOrder: QueryElement {
    public required Field Field { get; init; }
    public required SortOrderDirection Direction { get; init; }
}