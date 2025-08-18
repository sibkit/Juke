using Juke.Common;

namespace Juke.Querying;

// public enum ConditionType {
//     And,
//     Or,
//     Not,
//     Equals,
//     Greater,
//     GreaterOrEquals,
//     Less,
//     LessOrEquals,
//     NotEquals,
//     In,
//     Like
// }

public class AndCondition : Condition {
    public required IList<Condition> InnerConditions { get; init; }
}

public class OrCondition : Condition {
    public required IList<Condition> InnerConditions { get; init; }
}

public class NotCondition : Condition {
    public required Condition InnerCondition { get; init; }
}

public class EqualCondition : Condition {
    public EqualCondition(Field leftField, Field rightField) {
        leftField.Parent = this;
        rightField.Parent = this;
        LeftField = leftField;
        RightField = rightField;
    }
    public Field LeftField { get; }
    public Field RightField { get; }
}

public class GreaterCondition : Condition {
    public required Field LeftField { get; init; }
    public required Field RightField { get; init; }
}

public class GreaterOrEqualsCondition : Condition {
    public required Field LeftField { get; init; }
    public required Field RightField { get; init; }
}

public class LessCondition : Condition {
    public required Field LeftField { get; init; }
    public required Field RightField { get; init; }
}

public class LessOrEqualsCondition : Condition {
    public required Field LeftField { get; init; }
    public required Field RightField { get; init; }
}

public class NotEqualsCondition : Condition {
    public required Field LeftField { get; init; }
    public required Field RightField { get; init; }
}

public class InCondition : Condition {
    public required Field MainField { get; init; }
    public required IList<Field> SetFields { get; init; }
}

public class LikeCondition : Condition {
    public required Field LeftField { get; init; }
    public required Field RightField { get; init; }
}

public abstract class Condition: QueryElement {
    // public Condition(ConditionType type) {
    //     InnerConditions = new ChildList<QueryElement, Condition>(this);
    //     Fields = new ChildList<QueryElement, Field>(this);
    //     ConditionType = type;
    // }
    
    //public ConditionType ConditionType { get; set; } 
    //public IList<Condition> InnerConditions { get; }
    //public IList<Field> Fields { get; }
}