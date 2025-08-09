using Juke.Querying;

namespace Juke.Ado.Net.Sql;

public class LinkManager {
    private readonly MappingData _mappingData;
    //private readonly Dictionary<Query, List<NameTarget>> _namespaceMap = new();
    
    public LinkManager(MappingData mappingData) {
        _mappingData = mappingData;
    }

    public List<NameTarget> GetNamespace(Query query) {

        var space = new List<NameTarget>();
        switch (query) {
            case EntityQuery eq:
                break;
            case GroupQuery gq:
                space.AddRange(AvailableTargets(gq.Source));
                break;
            case JoinQuery jq:
                space.AddRange(AvailableTargets(jq.LeftSource));
                space.AddRange(AvailableTargets(jq.RightSource));
                break;
            default:
                throw new Exception("Unknown query type");
        }
        if (query.Parent is QueryField qf) {
            var q = FindOwnedQuery(qf);
            if (q == null) {
                throw new Exception("No owned query found");
            }
        }
        return space;
    }

    static Query? FindOwnedQuery(QueryElement? element) {
        return element switch {
            null => null,
            Query q => q,
            _ => FindOwnedQuery(element.Parent)
        };
    }
    
    
    public List<NameTarget> AvailableTargets(Source source)
    {
        List<NameTarget> result = [];
        if(source.Query.Fields.Count!=0)
        {
            foreach(var f in source.Query.Fields)
            {
                if(f.Alias is { Length: > 0 })
                {
                    result.Add(new NameTarget {
                        FieldName = f.Alias,
                        DbFieldName = null,
                        Source = source
                    });
                }
                else
                {
                    if (f is LinkField lf) {
                        result.Add(new NameTarget {
                            FieldName = lf.Name,
                            DbFieldName = null,
                            Source = source
                        });
                    }
                }

            }
            return result;
        }
        switch (source.Query) {
            case EntityQuery eq:
                var map = _mappingData.GetMapper(eq.EntityName).Map;
                foreach (var fm in map.FieldMaps) {
                    result.Add(new NameTarget {
                        FieldName = fm.FieldName,
                        DbFieldName = fm.DbColumnName,
                        Source = source
                    });
                }
                break;
            case JoinQuery jq:
                result.AddRange(AvailableTargets(jq.LeftSource));
                result.AddRange(AvailableTargets(jq.RightSource));
                break;
            default:
                throw new Exception("Unknown source query type");
        }
        return result;
    }
    
}