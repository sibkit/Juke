using System.Text;
using Juke.Querying;

namespace Juke.Sqlite.Sql;

public class SqlBuilder {
    public required MappingData MappingData { get; init; }
    
    private StringBuilder? BuildArray(string separator, Func<Field, StringBuilder?> build, IList<Field> fields) {
        return BuildArray(
            separator,
            build as Func<QueryElement, StringBuilder>
            ?? throw new InvalidOperationException(),
            fields as IList<QueryElement>
            ?? throw new InvalidOperationException());
    }

    private StringBuilder? BuildArray(string separator, Func<QueryElement, StringBuilder> build, IList<QueryElement> elements) {
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
    
    public StringBuilder BuildQuery(Query query) {
        switch (query) {
            case GroupQuery groupQuery: {
                var result = new StringBuilder("SELECT ");
                result.Append(BuildArray(", ", BuildField, groupQuery.Fields));
                result.Append(" FROM ");
                result.Append(BuildQuery(groupQuery.Source));
                result.Append(" GROUP BY ");
                //result.Append(BuildArray(", ", BuildField, groupQuery.GroupFields));
                if (groupQuery.Condition != null) {
                    result.Append(" HAVING ");
                    result.Append(BuildCondition(groupQuery.Condition));
                }
                return result;
            }
            case IEntityQuery entityQuery: {
                var map = MappingData.GetMapper(entityQuery.EntityType).Map;
                var result = new StringBuilder("SELECT ");
                
                if(entityQuery.Fields.Count==0) {
                    //var columnNames = map.FieldMaps.Select(x => x.DbColumnName);
                    result.Append('*');
                } else {
                    result.Append(BuildArray(", ", BuildField, entityQuery.Fields));
                }
                result.Append(" FROM ");
                result.Append(map.DbTableName);
                if (entityQuery.Condition != null) {
                    result.Append(" WHERE ");
                    result.Append(BuildCondition(entityQuery.Condition));
                }
                return result;
            }

            case JoinQuery joinQuery: {
                var result = new StringBuilder("SELECT ");
                result.Append(BuildArray(", ", BuildField, joinQuery.Fields));
                result.Append(" FROM ");
                result.Append(BuildQuery(joinQuery.LeftSource));
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
                result.Append(" JOIN ");
                return result;
            }
            default:
                throw new Exception("QuerySqlBuilder: BuildQuery(Unknown query type)");
        }
    }



    public StringBuilder? BuildField(Field field) {
        var result = new StringBuilder();
        switch (field) {
            case ValueField value:
                if(value.Value !=null)
                    result.Append($"'{value.Value}'");
                else 
                    result.Append("NULL");
                break;
            case LinkField link:
                //field.
                //link.Name
                break;
        }
        return result;
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