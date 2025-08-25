using Juke.Mapping;

namespace Juke.Accessing;


public interface IEntityOperation {
    Type EntityType {get; init;}
}

public class UpdateEntityOperation : IEntityOperation {
    public required Type EntityType {get; init;}
    public required EntityContent NewContent { get; init; }
    public required EntityContent OldContent { get; init; }
}

public class InsertEntityOperation : IEntityOperation {
    public required Type EntityType {get; init;}
    public required EntityContent Content { get; init; }
}

public class DeleteEntityOperation : IEntityOperation {
    public required Type EntityType {get; init;}
    public required EntityKey Key { get; init; }
}