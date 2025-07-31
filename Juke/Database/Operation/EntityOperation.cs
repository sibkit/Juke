using Juke.Mapping;

namespace Juke.Database.Operation;

public enum EntityOperationType {
    UPDATE,
    INSERT,
    DELETE
}

public class EntityOperation {
    public required EntityContent NewContent { get; init; }
    public required EntityContent OldContent { get; init; }
    public required EntityOperationType OperationType { get; init; }
}