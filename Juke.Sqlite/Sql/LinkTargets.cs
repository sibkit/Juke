using Juke.Mapping;
using Juke.Querying;

namespace Juke.Sqlite.Sql;

public interface ILinkTarget {
    
}

public class EntityTarget : ILinkTarget {
    public EntityTarget(IEntityQuery entityQuery, FieldMap fieldMap) {
        EntityQuery = entityQuery;
        FieldMap = fieldMap;
    }
    public IEntityQuery EntityQuery { get; }
    public FieldMap FieldMap { get; }
}

public class FieldTarget : ILinkTarget {
    public FieldTarget(Field field) {
        Field = field;
    }
    public Field Field { get; }
}