using Juke.Querying;

namespace Juke.Sqlite.Sql;

public interface ILinkTarget {
    
}

public class EntityTarget : ILinkTarget {
    public EntityTarget(IEntityQuery entityQuery) {
        EntityQuery = entityQuery;
    }
    public IEntityQuery EntityQuery { get; }
}

public class FieldTarget : ILinkTarget {
    public FieldTarget(Field field) {
        Field = field;
    }
    public Field Field { get; }
}