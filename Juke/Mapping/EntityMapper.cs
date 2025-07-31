namespace Juke.Mapping;


public interface IEntityMapper {
    object? ReadValue(object entity, FieldMap fieldMap);
    void WriteValue(object entity, FieldMap fieldMap, object? value);
    void BindToEntity(EntityContent content, object entity);
    void BindToContent(object entity, EntityContent content);
    
    EntityMap Map { get; }
}

public abstract class EntityMapper<T> : IEntityMapper 
    where T : class, new() {

    private EntityMap? _map;
    private Type _entityType;
    
    public abstract object? ReadValue(T entity, FieldMap fieldMap);
    public abstract void WriteValue(T entity, FieldMap fieldMap, object? value);

    protected abstract EntityMap CreateMap();
    
    public void BindToEntity(EntityContent content, T entity) {
        foreach (var fm in Map.FieldMaps) {
            WriteValue(entity, fm, content.GetFieldValue<object>(fm.Index));
        }
    }

    public void BindToContent(T entity, EntityContent content) {
        foreach (var fm in Map.FieldMaps) {
            content.SetFieldValue(fm.Index, ReadValue(entity, fm));
        }
    }

    public EntityMap Map {
        get { return _map ??= CreateMap(); }
    }

    object? IEntityMapper.ReadValue(object entity, FieldMap fieldMap) {
        return ReadValue((T)entity, fieldMap);
    }

    void IEntityMapper.WriteValue(object entity, FieldMap fieldMap, object? value) {
        WriteValue((T)entity, fieldMap, value);
    }

    void IEntityMapper.BindToEntity(EntityContent content, object entity) {
        BindToEntity(content, (T)entity);
    }
    void IEntityMapper.BindToContent(object entity, EntityContent content) {
        BindToContent((T)entity, content);
    }
}