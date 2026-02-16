using Juke.Common;

namespace Juke.Querying;

public interface IQuery {
    public string? Alias { get; set; }
    public IList<Field> Fields { get; }
    public int? Offset { get; set; }
    public int? Limit { get; set; }
    public Condition? Condition { get; set; }
    public IList<SortOrder> SortOrders { get; }
}


public abstract class Query : QueryElement, IQuery {
   private Condition? _condition;
    public string? Alias { get; set; }
    protected Query() {
        Fields = new ChildList<QueryElement, Field>(this);
        SortOrders = new ChildList<QueryElement, SortOrder>(this);
    }
    public IList<Field> Fields { get; }
    public int? Offset { get; set; } = null;
    public int? Limit { get; set; } = null;
    
    public Condition? Condition {
        get => _condition;
        set {
            if(value != null)
                value.Parent = this;
            _condition = value;
        }
    }
    
    public IList<SortOrder> SortOrders { get; }
}

public interface IEntityQuery: IQuery {
    public Type EntityType { get; }
}

public class EntityQuery<T>: Query, IEntityQuery
where T : class, new() {

    public Type EntityType => typeof(T);
}

public class GroupQuery: Query {
    // private Condition? _havingCondition;
    // public Condition? HavingCondition {
    //     get => _havingCondition;
    //     set {
    //         if(value != null)
    //             value.Parent = this;
    //         _havingCondition = value;
    //     }
    // }
    public IList<LinkField> GroupFields { get; }
    public required Query Source { get; set; }
    
    public GroupQuery() {
        GroupFields = new ChildList<QueryElement, LinkField>(this);
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
    // private Condition? _onCondition;
    // public Condition? OnCondition {
    //     get => _onCondition;
    //     set {
    //         if(value != null)
    //             value.Parent = this;
    //         _onCondition = value;
    //     }
    // }
    public JoinType JoinType { get; init; }
   
    public required Query LeftSource { get; init; }
    public required Query RightSource { get; init; }
}