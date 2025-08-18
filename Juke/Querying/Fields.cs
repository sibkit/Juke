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
    public string TargetFieldName { get; init; }
    public string? SourceAlias { get; init; }

    public LinkField(string targetFieldName) {
        TargetFieldName = targetFieldName;
    }
    
    public LinkField(string sourceAlias, string targetFieldName) {
        TargetFieldName = targetFieldName;
        SourceAlias = sourceAlias;
    }
    
    public override string ToString() {
        return SourceAlias!=null ? 
            $"{SourceAlias}.{TargetFieldName}" : 
            $"{TargetFieldName}";
    }
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
    public ValueField(object? value) {
        Value = value;
    }
    public object? Value { get; set; }
}