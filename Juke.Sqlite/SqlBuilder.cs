using System.Text;
using Juke.Ado.Net.Sql;
using Juke.Database;
using Juke.Mapping;
using Juke.Querying;
using AdoSqlite = Microsoft.Data.Sqlite;

namespace Juke.Sqlite;

public class SqlBuilder {
    
    public AdoSqlite.SqliteCommand BuildQueryCommand(Query query, AdoSqlite.SqliteConnection sqliteConnection) {
        
        throw new NotImplementedException();
    }
    
    public AdoSqlite.SqliteCommand BuildSequenceCommand(string sequenceName, AdoSqlite.SqliteConnection sqliteConnection) {
        
        throw new NotImplementedException();
    }
    
    public AdoSqlite.SqliteCommand BuildOperationCommand(IEntityOperation operation, AdoSqlite.SqliteConnection sqliteConnection) {
        switch (operation) {
            case InsertEntityOperation insert: {
                var map = insert.Content.EntityMap;
                var sb = new StringBuilder("INSERT INTO ");
                sb.Append(map.DbTableName);
                sb.Append(" (");
                 
                var columnNames = new List<string>();
                var values = new List<object>();
                foreach (var fm in insert.Content.EntityMap.FieldMaps) {
                    var fValue = insert.Content.GetFieldValue(fm.Index);
                    if (fValue != null) {
                        columnNames.Add(fm.DbColumnName);
                        values.Add(fm.ValueConverter == null ? fValue : fm.ValueConverter.convertToDb(fValue));
                    }
                }
                sb.AppendJoin(", ", columnNames);
                sb.Append(") VALUES (");
                sb.AppendJoin(", ", values);
                return new AdoSqlite.SqliteCommand(sb.ToString());
            }

            case UpdateEntityOperation update: {
                var map = update.NewContent.EntityMap;
                var sb = new StringBuilder("UPDATE ");
                return new AdoSqlite.SqliteCommand(sb.ToString());
            }

            case DeleteEntityOperation delete: {
                var map = delete.Key.EntityMap;
                var sb = new StringBuilder("DELETE FROM ");
                return new AdoSqlite.SqliteCommand(sb.ToString());
            }
            default:
                throw new Exception("Unknown operation type");
        }
    }
}