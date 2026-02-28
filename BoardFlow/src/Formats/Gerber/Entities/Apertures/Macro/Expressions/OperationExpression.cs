namespace BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;

public enum OperationType {
    Add,
    Subtract,
    Multiply,
    Divide,
}

public static class OperationTypeExtensions
{
    public static string ToOperationString(this OperationType me) {
        return me switch {
            OperationType.Add => "+",
            OperationType.Subtract => "-",
            OperationType.Multiply => "*",
            OperationType.Divide => "/",
            _ => "ERR"
        };
    }
}

public class OperationExpression : IExpression {
    public OperationExpression(
        OperationType operationType,
        IExpression? leftExpression,
        IExpression? rightExpression) 
    {
        OperationType = operationType;
        LeftExpression = leftExpression;
        RightExpression = rightExpression;
    }

    public OperationExpression(OperationType operationType) 
    {
        OperationType = operationType;
    }
    
    public OperationType OperationType { get; set; }
    public IExpression? LeftExpression { get; set; }
    public IExpression? RightExpression { get; set; }

    public override string ToString() {
        return "("+ LeftExpression + " " + OperationType.ToOperationString() + " " + RightExpression +")";
    }
}