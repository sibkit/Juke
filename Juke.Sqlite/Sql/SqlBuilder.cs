using System.Text;
using Juke.Exceptions;
using Juke.Querying;

namespace Juke.Sqlite.Sql;

public class SqlBuilder {
    public LinkManager LinkManager { get; }
    
    public MappingData MappingData { get; }
    public Query Query { get; }
    public SqlBuilder(Query query, MappingData mapping) {
        LinkManager = new LinkManager(query, mapping);
        MappingData = mapping;
        Query = query;
    }
    
    private StringBuilder? BuildArray<T>(string separator, Func<T, StringBuilder?> build, IList<T> elements) 
    where T: QueryElement {
        if (elements.Count == 0)
            return null;
        var result = new StringBuilder();
        result.Append(build(elements[0]));
        for (var i = 1; i < elements.Count; i++) {
            result.Append(separator);
            result.Append(build(elements[i]));
        }
        return result;
    }

    // private StringBuilder? BuildArray(string separator, Func<QueryElement, StringBuilder> build, IList<QueryElement> elements) {
    //     if (elements.Count == 0)
    //         return null;
    //     var result = new StringBuilder();
    //     result.Append(build(elements[0]));
    //     for (var i = 1; i < elements.Count; i++) {
    //         result.Append(separator);
    //         result.Append(build(elements[i]));
    //     }
    //     return result;
    // }

    private StringBuilder? BuildArray(string separator, IEnumerable<string> elements) {
        var list = elements.ToList();
        if (list.Count == 0)
            return null;
        var result = new StringBuilder();
        result.Append(list[0]);
        for (var i = 1; i < list.Count; i++) {
            result.Append(separator);
            result.Append(list[i]);
        }
        return result;
    }

    StringBuilder BuildInnerQuery(Query query) {
        var result = new StringBuilder("(");
        if (query.Alias == null) {
            result.Append(BuildQuery(query));
            result.Append(")");
            return result;
        }
        result.Append(BuildQuery(query));
        result.Append(") AS ");
        result.Append(query.Alias);
        return result;
    }
    
    public StringBuilder BuildQuery(Query query) {
        var result = new StringBuilder("SELECT ");
        switch (query) {
            case GroupQuery groupQuery: {
                result.Append(BuildArray(", ", BuildField, groupQuery.Fields));
                result.Append(" FROM ");
                result.Append(BuildInnerQuery(groupQuery.Source));
                if (groupQuery.GroupFields.Count > 0) {
                    result.Append(" GROUP BY ");
                    result.Append(BuildArray(", ", BuildField, groupQuery.GroupFields));
                }

                if (groupQuery.Condition != null) {
                    result.Append(" HAVING ");
                    result.Append(BuildCondition(groupQuery.Condition));
                }
                break;
            }
            case IEntityQuery entityQuery: {
                var map = MappingData.GetMapper(entityQuery.EntityType).Map;
                
                if(entityQuery.Fields.Count==0) {
                    var columnNames = map.FieldMaps.Select(x => x.DbColumnName);
                    result.Append(BuildArray(", ", columnNames));
                } else {
                    result.Append(BuildArray(", ", BuildField, entityQuery.Fields));
                }
                result.Append(" FROM ");
                result.Append(map.DbTableName);
                if (entityQuery.Condition != null) {
                    result.Append(" WHERE ");
                    result.Append(BuildCondition(entityQuery.Condition));
                }
                break;
            }

            case JoinQuery joinQuery: {
                result.Append(BuildArray(", ", BuildField, joinQuery.Fields));
                result.Append($" FROM ({BuildInnerQuery(joinQuery.LeftSource)})");

                //result.Append();
                switch (joinQuery.JoinType) {
                    case JoinType.Cross:
                        result.Append(" CROSS JOIN ");
                        break;
                    case JoinType.Inner:
                        result.Append(" INNER JOIN ");
                        break;
                    case JoinType.LeftOuter:
                        result.Append(" LEFT JOIN ");
                        break;
                    case JoinType.RightOuter:
                        result.Append(" RIGHT JOIN ");
                        break;
                    case JoinType.FullOuter:
                        result.Append(" FULL JOIN ");
                        break;
                    default:
                        throw new Exception("QuerySqlBuilder: BuildQuery(Unknown JoinType)");
                }
                result.Append(BuildInnerQuery(joinQuery.RightSource));
                break;
            }
            default:
                throw new Exception("QuerySqlBuilder: BuildQuery(Unknown query type)");
        }
        if (query.SortOrders.Count > 0) {
            result.Append(" ORDER BY ");
            result.Append(BuildArray(", ",BuildSortOrder, query.SortOrders));
        }
        if (query.Limit != null) {
            result.Append(" LIMIT ");
            result.Append(query.Limit);
        }
        if (query.Offset != null) {
            result.Append(" OFFSET ");
            result.Append(query.Offset);
        }
            
        return result;
    }

    public StringBuilder? BuildSortOrder(SortOrder sortOrder) {
        var result = BuildField(sortOrder.Field);
        if(sortOrder.Direction == SortOrderDirection.Desc)
            result?.Append(" DESC");
        return result;
    }

    public StringBuilder BuildField(Field field) {
        var result = new StringBuilder();
        switch (field) {
            case ValueField value:
                if(value.Value !=null)
                    result.Append($"'{value.Value}'");
                else 
                    result.Append("NULL");
                break;
            case LinkField link:

                var target = LinkManager.FindTarget(link);
                if (target == null) {
                    throw new JukeException("QuerySqlBuilder: LinkField target not found");
                }
                switch (target) {
                    case FieldTarget ft:
                        if(ft.Field.Alias!=null)
                            result.Append($"{ft.Field.Alias}");
                        else {
                            if (ft.Field is LinkField lf) {
                                result.Append(BuildField(lf));
                            } else {
                                throw new Exception("SqlBuilder: BuildField (LinkField target error)");
                            }
                        }
                        break;
                    case EntityTarget et:
                        if(et.EntityQuery.Alias!=null)
                            result.Append($"{et.EntityQuery.Alias}.");
                        result.Append($"{et.FieldMap.DbColumnName}");
                        break;
                    default:
                        throw new Exception("SqlBuilder: BuildField (Unknown FieldTarget)");
                }
                if(target == null)
                    throw new Exception("SqlBuilder: BuildField (Target not found)");
                break;
            case IFunctionField ff:
                result.Append(BuildFunctionField(ff));
                break;
            case QueryField qf:
                result.Append(BuildInnerQuery(qf.Query));
                break;
            default:
                throw new Exception("SqlBuilder: BuildField(Unknown field type)");
        }
        return result;
    }

    public string BuildFunctionField(IFunctionField ff) {
        return ff switch {
            SumField sum => $"SUM({BuildField(sum.NestedField)})",
            CountField count => $"COUNT({BuildField(count.NestedField)})",
            UpperField upper => $"UPPER({BuildField(upper.NestedField)})",
            LowerField lower => $"LOWER({BuildField(lower.NestedField)})",
            _ => throw new Exception("SqlBuilder: BuildFunctionField (Unknown FunctionField type)")
        };
    }
    
    public StringBuilder BuildCondition(Condition condition) {
        var result = new StringBuilder();
        switch (condition) {
            case LikeCondition like:
                result.Append(BuildField(like.LeftField));
                result.Append(" LIKE ");
                result.Append(BuildField(like.RightField));
                break;
            default:
                throw new Exception("CommandBuilder: BuildCondition (Unknown condition type)");
        }
        return result;
    }
}