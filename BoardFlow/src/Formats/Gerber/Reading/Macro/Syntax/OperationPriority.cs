using System;
using BoardFlow.Formats.Gerber.Entities.Apertures.Macro.Expressions;

namespace BoardFlow.Formats.Gerber.Reading.Macro.Syntax;

public enum OperationPriority {
    P1 = 1,
    P2 = 2
}

public static class OperationPriorityExtension {
    public static OperationPriority GetPriority(this OperationType type) {
        return type switch {
            OperationType.Add => OperationPriority.P1,
            OperationType.Subtract => OperationPriority.P1,
            OperationType.Multiply => OperationPriority.P2,
            OperationType.Divide => OperationPriority.P2,
            _ => throw new Exception("Unknown operation type")
        };
    }
}

