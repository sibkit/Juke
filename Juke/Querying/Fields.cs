using Juke.Common;

namespace Juke.Querying;

public abstract class Field : QueryElement {
    public string? Alias { get; set; }
}

public enum FunctionType {
    Count,
    Avg,
    Max,
    Min,
    Sum,
    RowNumber,
    Lower,
    Upper
}

public class FunctionField : Field {
    public FunctionField() {
        NestedFields = new ChildList<QueryElement, Field>(this);
    }
    public required FunctionType Type { get; init; }
    public IList<Field> NestedFields { get; }
}

public class LinkField : Field {
    public required string Name { get; init; }
    public string? SourceAlias { get; init; }
}

public class QueryField : Field {
    private Query? _query;
    public Query? Query {
        get => _query;
        set {
            if (_query != null) _query.Parent = null;
            _query = value;
            if (_query != null) _query.Parent = this;
        }
    }
}

public class ValueField : Field {
    public object? Value { get; set; }
}