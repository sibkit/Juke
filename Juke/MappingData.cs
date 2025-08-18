using Juke.Mapping;
using Juke.Querying;

namespace Juke;

public class MappingData {
    private readonly Dictionary<string, IEntityMapper> mappersByName = new();
    private readonly Dictionary<Type, IEntityMapper> mappersByType = new();
    private readonly Dictionary<string, SequenceMap> sequencesMap = new();
    
    public IReadOnlyCollection<IEntityMapper> Mappers => mappersByName.Values;

    public void RegisterSequence(SequenceMap sequenceMap) {
        sequencesMap.Add(sequenceMap.SequenceName, sequenceMap);
    }

    public SequenceMap SequenceMap(string sequenceName) {
        return sequencesMap[sequenceName];
    }

    public void AddMapper(IEntityMapper mapper) {
        mappersByName.Add(mapper.Map.EntityName, mapper);
        mappersByType.Add(mapper.EntityType, mapper);
    }

    public void RemoveMapper(IEntityMapper mapper) {
        mappersByName.Remove(mapper.Map.EntityName);
        mappersByType.Remove(mapper.EntityType);
    }

    public IEntityMapper GetMapper(string entityName) {
        return mappersByName[entityName];
    }

    public IEntityMapper GetMapper(Type entityType) {
        return mappersByType[entityType];
    }

    // public void CompleteQuery(Query query) {
    //     if (query is IEntityQuery { Fields.Count: 0 } eq) {
    //         var mapper = mappersByName[eq.EntityName];
    //         foreach (var fieldName in mapper.Map.FieldNames) {
    //             eq.Fields.Add(new LinkField {
    //                 Name = fieldName
    //             });
    //         }
    //     }
    // }
}