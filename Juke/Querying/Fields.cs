using Juke.Common;

namespace Juke.Querying;

public abstract class Field : QueryElement {
    public string? Alias { get; set; }
}



public interface IFunctionField { }

public class AvgField : Field, IFunctionField {
    public Field NestedField { get; }

    public AvgField(Field nestedField) {
        nestedField.Parent = this;
        NestedField = nestedField;
    }
}

public class MaxField : Field, IFunctionField {
    public Field NestedField { get; }

    public MaxField(Field nestedField) {
        nestedField.Parent = this;
        NestedField = nestedField;
    }
}

public class MinField : Field, IFunctionField {
    public Field NestedField { get; }

    public MinField(Field nestedField) {
        nestedField.Parent = this;
        NestedField = nestedField;
    }
}

public class SumField : Field, IFunctionField {
    public Field NestedField { get; }

    public SumField(Field nestedField) {
        nestedField.Parent = this;
        NestedField = nestedField;
    }
}

public class RowNumberField : Field, IFunctionField {}

public class CountField : Field, IFunctionField {
    public Field NestedField { get; }

    public CountField(Field nestedField) {
        nestedField.Parent = this;
        NestedField = nestedField;
    }
}

public class UpperField : Field, IFunctionField {
    public Field NestedField { get; }

    public UpperField(Field nestedField) {
        nestedField.Parent = this;
        NestedField = nestedField;
    }
}

public class LowerField : Field, IFunctionField {
    public Field NestedField { get; }

    public LowerField(Field nestedField) {
        nestedField.Parent = this;
        NestedField = nestedField;
    }
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

    public QueryField(Query query) {
        query.Parent = this;
        Query = query;
    }

    public Query Query { get; }
}

public class ValueField : Field {
    public ValueField(object? value) {
        Value = value;
    }
    public object? Value { get; set; }
}