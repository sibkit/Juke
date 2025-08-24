using Juke.Querying;

namespace Juke.Sqlite.Sql;

public class LinkManager {
    public Query Query { get; }
    public MappingData Mapping { get; }

    public LinkManager(Query query, MappingData mapping) {
        Query = query;
        Mapping = mapping;
    }

    private static Query? FindOwnedQuery(QueryElement? element) {
        return element switch {
            null => null,
            Query q => q,
            _ => FindOwnedQuery(element.Parent)
        };
    }

    static FieldTarget? FindTarget(LinkField field, IList<Field> fields) {
        foreach (var gqf in fields) {
            if (field == gqf)
                continue;
            if (field.TargetFieldName == gqf.Alias)
                return new FieldTarget(gqf);
            if (gqf is LinkField lf) {
                if (lf.Alias != null)
                    continue;
                if (lf.TargetFieldName == field.TargetFieldName)
                    return new FieldTarget(lf);
            }
        }
        return null;
    }

    public ILinkTarget? FindTarget(LinkField field) {
        var query = FindOwnedQuery(field);
        if (query == null)
            throw new Exception("Query not found");
        return FindTargetInSources(field, query);
    }

    ILinkTarget? FindTargetInFields(LinkField field, Query query) {
        //ILinkTarget? result = null;
        if (field.SourceAlias != null && field.SourceAlias != query.Alias)
            return null;
        if (query is IEntityQuery eq && (field.SourceAlias == null || field.SourceAlias == query.Alias)) {
            var mapper = Mapping.GetMapper(eq.EntityType);
            if (mapper == null)
                throw new Exception($"LinkManager: FindTargetQuery (Unregistered entity type: '{eq.EntityType.Name}')");
            foreach (var fm in mapper.Map.FieldMaps) {
                if(fm.FieldName == field.TargetFieldName)
                    return new EntityTarget(eq, fm);
            }
                
        }
        if (query.Fields.Count > 0) {
            foreach (var f in query.Fields) {
                if (f.Alias == field.TargetFieldName)
                    return new FieldTarget(f);
                if (f is LinkField lf) {
                    if (lf.Alias == null && lf.TargetFieldName == field.TargetFieldName)
                        return new FieldTarget(lf);
                }
            }
        }
        return null;
    }

    ILinkTarget? FindTargetInSources(LinkField field, Query query) {
        ILinkTarget? result;
        switch (query) {
            case GroupQuery gq:
                result = FindTargetInFields(field, gq.Source);
                break;
            case JoinQuery jq:
                result = FindTargetInFields(field, jq.LeftSource);
                result =  result ?? FindTargetInFields(field, jq.RightSource);
                break;
            case IEntityQuery eq:
                if (field.SourceAlias == null || field.SourceAlias == query.Alias) {
                    var mapper = Mapping.GetMapper(eq.EntityType);
                    if (mapper == null)
                        throw new Exception($"LinkManager: FindTargetQuery (Unregistered entity type: '{eq.EntityType.Name}')");
                    foreach (var fm in mapper.Map.FieldMaps) {
                        if(fm.FieldName == field.TargetFieldName)
                            return new EntityTarget(eq, fm);
                    }
                
                }
                result = null;
                break;
            default:
                throw new Exception("Unknown query type");
        }
        if (result == null && query.Parent is QueryField qf) {
            var parentQuery = FindOwnedQuery(qf);
            if (parentQuery != null)
                return FindTargetInFields(field, parentQuery);
        }
        return result;
    }
    
    // ILinkTarget? FindTarget(LinkField field, Query query) {
    //     ILinkTarget? result;
    //     switch (query) {
    //         case IEntityQuery eq: {
    //             if (field.SourceAlias!=null && field.SourceAlias != query.Alias)
    //                 return null;
    //             var mapper = Mapping.GetMapper(eq.EntityType);
    //             if (mapper == null)
    //                 throw new Exception($"LinkManager: FindTargetQuery (Unregistered entity type: '{eq.EntityType.Name}')");
    //             result = new EntityTarget(eq);
    //             break;
    //         }
    //         case GroupQuery gq: {
    //             if (field.SourceAlias != null && field.SourceAlias != gq.Alias) {
    //                 return null;
    //             }
    //                 
    //             result = FindTarget(field, gq.Fields);
    //             result = result ?? FindTarget(field, gq.Source);
    //             break;
    //         }
    //         case JoinQuery jq: {
    //             if (jq.Fields.Count > 0) {
    //                 if (field.SourceAlias != null && field.SourceAlias != jq.Alias)
    //                     return null;
    //                 result = FindTarget(field, jq.Fields);
    //             } else {
    //                 result = FindTarget(field, jq.LeftSource);
    //                 result = result ?? FindTarget(field, jq.RightSource);
    //             }
    //             break;
    //         }
    //         default:
    //             throw new Exception("Unknown query type");
    //     }
    //     if (result == null && query.Parent is QueryField qf) {
    //         query = FindOwnedQuery(qf);
    //         if (query == null)
    //             throw new Exception("Query not found");
    //         return FindTarget(field, query);
    //     }
    //     return result;
    // }

    // static List<Query> GetSourceQueries(JoinQuery jq) {
    //     var result = new List<Query>();
    //     if (jq.LeftSource is JoinQuery jqLeft) {
    //         if (jqLeft.Alias is { Length: > 0 }) {
    //             if (jqLeft.Fields.Count > 0)
    //                 result.Add(jqLeft);
    //             else {
    //                 throw new Exception("Join query have alias, but not have any fields");
    //             }
    //         } else {
    //             result.AddRange(GetSourceQueries(jqLeft));
    //         }
    //     }
    //     if (jq.RightSource is JoinQuery jqRight) {
    //         if (jqRight.Alias is { Length: > 0 }) {
    //             if (jqRight.Fields.Count > 0)
    //                 result.Add(jqRight);
    //             else {
    //                 throw new Exception("Join query have alias, but not have any fields");
    //             }
    //         } else {
    //             result.AddRange(GetSourceQueries(jqRight));
    //         }
    //     }
    //     return result;
    // }

    
    
    // public Query FindTargetQuery(LinkField field) {
    //     var query = FindOwnedQuery(field);
    //     switch (query) {
    //         case IEntityQuery eq: {
    //             if (field.SourceAlias != null)
    //                 throw new Exception("LinkManager: FindTargetQuery (invalid field source alias)");
    //             var mapper = Mapping.GetMapper(eq.EntityType);
    //             if (mapper == null)
    //                 throw new Exception($"LinkManager: FindTargetQuery (Unregistered entity type: '{eq.EntityType.Name}')");
    //             return query;
    //         }
    //         case JoinQuery jq: {
    //             var queries = GetSourceQueries(jq);
    //             foreach (var q in queries) {
    //                 if (q.Alias != field.SourceAlias)
    //                     continue;
    //                 foreach (var qField in q.Fields) {
    //                     if (qField.Alias != null) {
    //                         if (field.TargetFieldName == qField.Alias)
    //                             return q;
    //                         continue;
    //                     }
    //                     if (qField is LinkField qlf) {
    //                         if (field.TargetFieldName == qlf.TargetFieldName) {
    //                             return q;
    //                         }
    //                     }
    //
    //                 }
    //             }
    //             return jq;
    //         }
    //         case GroupQuery gq: {
    //             if(field.SourceAlias != gq.Alias)
    //                 throw new Exception("LinkManager: FindTargetQuery (invalid field source alias)");
    //             return gq;
    //         }
    //         default:
    //             throw new Exception("LinkManager: FindTargetQuery (unknown query)");
    //     
    //     }
    // }
}