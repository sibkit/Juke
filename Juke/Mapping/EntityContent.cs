namespace Juke.Mapping;

public class EntityContent {

    public EntityContent(EntityMap entityMap) {
        EntityMap = entityMap;
        _fieldValues  = new object?[entityMap.FieldMaps.Length];
    }
    
    public EntityMap EntityMap {
        get;
    }

    private readonly object?[] _fieldValues;

    public EntityKey ExcludeKey() {
        var keyIndexes = EntityMap.KeyIndexes;
        if (keyIndexes.Length == 0) 
            throw new Exception("Key is not specified for entity: " + EntityMap.EntityName);
        
        var values = new KeyValue[keyIndexes.Length];
        for (var i = 0; i < keyIndexes.Length; i++) {
            var ki =  keyIndexes[i];
            var kfm = EntityMap.FieldMaps[ki];
            values[i] = new KeyValue {
                FieldMap = kfm,
                Value = _fieldValues[ki]
            };
        }
        return new EntityKey {
            Values = values,
            EntityMap = EntityMap
        };
    }

    public void SetFieldValue(int fieldIndex, object? value) {
        _fieldValues[fieldIndex] = value;
    }

    public object? GetFieldValue(int fieldIndex) {
        return _fieldValues[fieldIndex];
    }

    public override bool Equals(object? obj) {
        if (obj is EntityContent content) {
            return Equals(content);
        }
        return false;
    }

    protected bool Equals(EntityContent other) {
        if(other.EntityMap != EntityMap) 
            return false;
        for (var i = 0; i < _fieldValues.Length; i++) {
            if(_fieldValues[i] == null && other._fieldValues[i] != null)
                return false;
        }
        return true;
    }

    public override int GetHashCode() {
        return _fieldValues.GetHashCode();
    }
}