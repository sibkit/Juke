using System;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;
using BoardFlow.Formats.Gerber.Reading.Macro.Tokenize;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Syntax;

public class SyntaxOperator(OperationType type) : ISyntaxExpressionPart {
    public OperationType OperationType { get; } = type;
    public OperationPriority Priority {
        get {
            return OperationType switch {
                OperationType.Add or OperationType.Subtract => OperationPriority.P1,
                OperationType.Multiply or OperationType.Divide => OperationPriority.P2,
                _ => throw new Exception("Unknown operation type")
            };
        }
    }
    public IToken? Token { get; set; }
}
