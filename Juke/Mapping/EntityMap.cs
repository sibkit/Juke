namespace Juke.Mapping;

public class EntityMap {
    private readonly Dictionary<string, int> _fieldIndexes = new();
    public int[] KeyIndexes { get; private set; }
    private string[]? _fieldNames;

    private readonly FieldMap[] _fieldMaps;
    
    public required FieldMap[] FieldMaps {
        get => _fieldMaps;
        init {
            _fieldMaps = value;
            List<int> keys = [];
            for (var i = 0; i < _fieldMaps.Length; i++) {
                var fm = _fieldMaps[i];
                _fieldIndexes.Add(fm.FieldName, i);
                //fm.EntityMap = this;
                fm.Index = i;
                if(fm.IsKeyField)
                    keys.Add(i);
            }
            KeyIndexes = keys.ToArray();
        }
    }

    public string[] FieldNames {
        get {
            if (_fieldNames is null) {
                _fieldNames = new string[FieldMaps.Length];
                for (var i = 0; i < FieldMaps.Length; i++) {
                    _fieldNames[i] = FieldMaps[i].FieldName;
                }
            }
            return _fieldNames;
        }
    }
    
    public required string DbTableName { get; init; }
    public required string EntityName { get; init; }

    // public void SetFieldMaps(IList<FieldMap> fieldMaps) {
    //     FieldMaps = fieldMaps.ToArray();
    //     List<int> keys = [];
    //     for (var i = 0; i < FieldMaps.Length; i++) {
    //         var fm = FieldMaps[i];
    //         _fieldIndexes.Add(fm.FieldName, i);
    //         //fm.EntityMap = this;
    //         fm.Index = i;
    //         if(fm.IsKeyField)
    //             keys.Add(i);
    //     }
    //     KeyIndexes = keys.ToArray();
    // }

    public FieldMap FieldMap(string fieldName) {
        if (_fieldIndexes.TryGetValue(fieldName, out var index)) {
            return FieldMaps?[index] ?? throw new Exception("EntityMap: FieldMap not found");
        }
        throw new KeyNotFoundException($"Field {fieldName} not found");
    }
    

    
    
}