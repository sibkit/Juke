using Juke.Common;

namespace Juke.Querying;

public enum ConditionType {
    And,
    Or,
    Not,
    Equals,
    Greater,
    GreaterOrEquals,
    Less,
    LessOrEquals,
    NotEquals,
    In,
    Like
}

public class Condition: QueryElement {
    public Condition(ConditionType type) {
        InnerConditions = new ChildList<QueryElement, Condition>(this);
        Fields = new ChildList<QueryElement, Field>(this);
        ConditionType = type;
    }
    
    public ConditionType ConditionType { get; set; } 
    public IList<Condition> InnerConditions { get; }
    public IList<Field> Fields { get; }
}