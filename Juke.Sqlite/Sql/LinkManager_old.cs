// using Juke.Querying;
//
// namespace Juke.Sqlite.Sql;
//
// /// <summary>
// /// Analyzes the query and determines valid targets for fields of type LinkField
// /// </summary>
// public class LinkManager {
//     private readonly MappingData _mappingData;
//     public Query RootQuery { get; }
//     
//     private readonly Dictionary<IQuery, List<FieldTarget>> _presentTargetMaps = new();
//     private readonly Dictionary<IQuery, List<FieldTarget>> _validTargetMaps = new();
//     
//     public LinkManager(MappingData mappingData, Query query) {
//         _mappingData = mappingData;
//         RootQuery = query;
//     }
//
//
//     
//     private (string? sourceAlias, string name, string? alias) GetDbFieldName(LinkField linkField) {
//
//         (string? sourceAlias, string name, string? alias) result = new();
//         if (linkField.Alias is {Length:>0})
//             result.alias = linkField.Alias;
//         
//         var ownedQuery = FindOwnedQuery(linkField);
//         switch (ownedQuery) {
//             case IEntityQuery eq:
//                 
//                 break;
//         }
//         return result;
//     }
//
//     private IQuery? FindTargetQuery(IQuery query, string? sourceAlias, string name) {
//         switch (query) {
//             case IEntityQuery eq:
//                 if(eq.Alias is {Length:>0} && eq.Alias == query.Alias)
//                     return eq;
//                 
//                 break;
//         }
//     }
//     
//     public void FillTargetMaps(IQuery query) {
//         if (_presentTargetMaps.TryGetValue(query, out List<FieldTarget> targets)) {
//             if(targets.Contains())
//         }
//         var nt = new FieldTarget {
//             Consumer = null,
//             Provider = query,
//             QueryName = 
//         }
//         
//     }
//     
//     
//     
//     public FieldTarget FindTarget(LinkField linkField) {
//         var query = FindOwnedQuery(linkField);
//         if (query == null) {
//             throw new Exception("LinkManager: GetDbTargetName (Query not found)");
//         }
//         foreach (var nt in GetValidTargets(query)) {
//             if(nt.TargetName!=linkField.TargetFieldName)
//                 continue;
//             if (nt.Source.Alias == linkField.SourceAlias) {
//                 return nt;
//             }
//         }
//         throw new Exception($"LinkManager: FindTarget (for field: '{linkField}' Target not found)");
//     }
//     
//     // private (string dbName, string? dbSourceAlias) GetDbTargetName(LinkField linkField) {
//     //     
//     //     var nt = FindTarget(linkField);
//     //     switch (nt.Source) {
//     //         case IEntityQuery eq: {
//     //             if (_dbAliasesMap.TryGetValue((eq.EntityType, eq.Alias), out var dbAlias)) {
//     //                 return (nt.DbTargetName, dbAlias);
//     //             }
//     //             //Alias not true 
//     //             _dbAliasesMap.Add((eq.EntityType, eq.Alias), eq.Alias);
//     //             return (nt.DbTargetName, eq.Alias);
//     //         }
//     //         case GroupQuery gc:
//     //             return (nt.DbTargetName, gc.Alias);
//     //         case JoinQuery jq:
//     //             return (nt.DbTargetName, jq.Alias);
//     //         default:
//     //             throw new Exception($"LinkManager: GetDbTargetName (Unknown query type)");
//     //     }
//         // if (linkField.SourceAlias != null) {
//         //
//         //
//         // }
//         // var query = FindOwnedQuery(linkField);
//         // if (query == null) {
//         //     throw new Exception("LinkManager: GetDbTargetName (Query not found)");
//         // }
//         // foreach (var nt in GetValidTargets(query)) {
//         //     if(nt.TargetName!=linkField.TargetFieldName)
//         //         continue;
//         //     if (nt.Source.Alias != null) {
//         //         
//         //     }
//         // }
//     //}
//     
//     public List<NameTargetMap> GetValidTargetMaps(Query query) {
//         if (_validTargetMaps.TryGetValue(query, out var targetMaps)) 
//             return targetMaps;
//         
//         
//         var result = new List<NameTargetMap>();
//         switch (query) {
//             case IEntityQuery eq:
//                 result.AddRange(GetIncomeTargetMaps(eq));
//                 break;
//             case GroupQuery gq:
//                 result.AddRange(GetPresentedTargets(gq.Source));
//                 break;
//             case JoinQuery jq:
//                 result.AddRange(GetPresentedTargets(jq.LeftSource));
//                 result.AddRange(GetPresentedTargets(jq.RightSource));
//                 break;
//             default:
//                 throw new Exception("Unknown query type");
//         }
//         if (query.Parent is QueryField qf) {
//             var parentQuery = FindOwnedQuery(qf);
//             if (parentQuery == null) {
//                 throw new Exception("No owned query found");
//             }
//             result.AddRange(GetValidTargets(parentQuery));
//         }
//         _validTargets.Add(query, result);
//         return result;
//     }
//
//     private static Query? FindOwnedQuery(QueryElement? element) {
//         return element switch {
//             null => null,
//             Query q => q,
//             _ => FindOwnedQuery(element.Parent)
//         };
//     }
//
//     private List<NameTargetMap> GetIncomeTargetMaps(Type entityType) {
//         var map = _mappingData.GetMapper(entityType).Map;
//         List<NameTargetMap> result = [];
//         foreach (var fm in map.FieldMaps) {
//             result.Add(new NameTargetMap {
//                 QueryNameTarget = new FieldTarget(fm.FieldName),
//                 DatabaseNameTarget = new FieldTarget(fm.DbColumnName)
//             });
//         }
//         return result;
//     }
//
//
//     
//     public List<NameTargetMap> GetPresentedTargets(Query query) {
//         var sourceAlias = query.Alias is { Length: > 0 } ? query.Alias : null;
//         
//         if (query.Fields.Count != 0) {
//             List<NameTargetMap> result = [];
//             foreach (var f in query.Fields) {
//
//                 if (f.Alias is { Length: > 0 }) {
//                     result.Add(new NameTargetMap {
//                         QueryNameTarget = new FieldTarget(sourceAlias, f.Alias),
//                         DatabaseNameTarget = new FieldTarget(sourceAlias, f.Alias)
//                     });
//                 } else {
//                     if (f is LinkField lf) {
//                         result.Add(new NameTargetMap {
//                             QueryNameTarget = new FieldTarget(sourceAlias, lf.TargetFieldName),
//                             DatabaseNameTarget = new FieldTarget(sourceAlias, f.Alias)
//                         });
//                     }
//                 }
//             }
//             return result;
//         }
//         
//         switch (query) {
//             case IEntityQuery eq: {
//                 return GetIncomeTargetMaps(eq);
//             }
//             case JoinQuery jq: {
//                 List<FieldTarget> result = [];
//                 result.AddRange(GetPresentedTargets(jq.LeftSource));
//                 result.AddRange(GetPresentedTargets(jq.RightSource));
//                 return result;
//             }
//             case GroupQuery gq: {
//                 List<FieldTarget> result = [];
//                 result.AddRange(gq.GroupFields);
//             }
//             default:
//                 throw new Exception("Unknown source query type");
//         }
//     }
// }