using Juke.Mapping;

namespace Juke.Database;


public interface IEntityOperation {}

public class UpdateEntityOperation : IEntityOperation {
    public required EntityContent NewContent { get; init; }
    public required EntityContent OldContent { get; init; }
}

public class InsertEntityOperation : IEntityOperation {
    public required EntityContent Content { get; init; }
}

public class DeleteEntityOperation : IEntityOperation {
    public required EntityKey Key { get; init; }
}