using Juke.Common;

namespace Juke.Querying;

public abstract class Query : QueryElement {
    protected Query() {
        Fields = new ChildList<QueryElement, Field>(this);
        SortOrders = new ChildList<QueryElement, SortOrder>(this);
    }
    public IList<Field> Fields { get; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    public Condition? Condition { get; set; }
    public IList<SortOrder> SortOrders { get; }
}

public class EntityQuery: Query {
    public required string EntityName { get; init; }
}

public class GroupQuery: Query {
    public IList<Field> GroupFields { get; }
    public required Source Source { get; set; }
    
    public GroupQuery() {
        GroupFields = new ChildList<QueryElement, Field>(this);
    }
}

public enum JoinType {
    Cross,
    Inner,
    LeftOuter,
    RightOuter,
    FullOuter
}

public class JoinQuery: Query {
    public JoinType JoinType { get; init; }
    public Condition? OnCondition { get; init; }
    public required Source LeftSource { get; init; }
    public required Source RightSource { get; init; }
}