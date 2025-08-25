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

public abstract class LeftRightCondition : Condition {
    protected LeftRightCondition(Field leftField, Field rightField) {
        leftField.Parent = this;
        rightField.Parent = this;
        LeftField = leftField;
        RightField = rightField;
    }
    public Field LeftField { get; }
    public Field RightField { get; }
}

public class EqualCondition : LeftRightCondition {
    public EqualCondition(Field leftField, Field rightField) : base(leftField, rightField) { }
}

public class GreaterCondition : LeftRightCondition {
    public GreaterCondition(Field leftField, Field rightField) : base(leftField, rightField) { }
}

public class GreaterOrEqualsCondition : LeftRightCondition {
    public GreaterOrEqualsCondition(Field leftField, Field rightField) : base(leftField, rightField) { }
}

public class LessCondition : LeftRightCondition {
    public LessCondition(Field leftField, Field rightField) : base(leftField, rightField) { }
}

public class LessOrEqualsCondition : LeftRightCondition {
    public LessOrEqualsCondition(Field leftField, Field rightField) : base(leftField, rightField) { }
}

public class NotEqualsCondition : LeftRightCondition {
    public NotEqualsCondition(Field leftField, Field rightField) : base(leftField, rightField) { }
}

public class InCondition : Condition {
    public required Field MainField { get; init; }
    public required IList<Field> SetFields { get; init; }
}

public class LikeCondition : LeftRightCondition {
    public LikeCondition(Field leftField, Field rightField) : base(leftField, rightField) { }
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