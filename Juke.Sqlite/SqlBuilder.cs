using System.Collections.ObjectModel;
using System.Text;
using Juke.Accessing;
using Juke.Ado.Net.Sql;
using Juke.Mapping;
using Juke.Querying;
using AdoSqlite = Microsoft.Data.Sqlite;

namespace Juke.Sqlite;

public class SqlBuilder {
    
    public AdoSqlite.SqliteCommand BuildQueryCommand(Query query, AdoSqlite.SqliteConnection sqliteConnection) {
        
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
                sb.AppendJoin(", '", values);
                sb.Append("')");
                var cmd = sqliteConnection.CreateCommand();
                cmd.CommandText = sb.ToString();

                return cmd;
            }

            case UpdateEntityOperation update: {
                var map = update.NewContent.EntityMap;

                if (map.KeyIndexes.Length == 0)
                    throw new Exception("No key index found");

                var sb = new StringBuilder("UPDATE ");
                sb.Append(map.DbTableName);
                sb.Append(" SET ");

                var updateParts = new List<string>();
                for (var i = 0; i < update.NewContent.EntityMap.FieldNames.Length; i++) {
                    if (update.OldContent.GetFieldValue(i) != update.NewContent.GetFieldValue(i)) {
                        updateParts.Add(update.NewContent.EntityMap.FieldNames[i] + " = " + update.NewContent.GetFieldValue(i));
                    }
                }

                sb.AppendJoin(", ", updateParts);

                sb.Append(" WHERE ");
                var keys = new List<string>(map.KeyIndexes.Length);
                foreach (var ki in map.KeyIndexes) {
                    keys.Add(map.FieldNames[ki] + " = " + update.NewContent.GetFieldValue(ki));
                }
                sb.AppendJoin(", ", keys);


                var cmd = sqliteConnection.CreateCommand();
                cmd.CommandText = sb.ToString();

                return cmd;
            }

            case DeleteEntityOperation delete: {
                var map = delete.Key.EntityMap;
                var sb = new StringBuilder("DELETE FROM ");
                sb.Append(map.DbTableName);
                sb.Append(" WHERE ");
                var keys = new List<string>(map.KeyIndexes.Length);
                foreach (var kv in delete.Key.Values) {
                    keys.Add(kv.FieldMap.DbColumnName + " = " + kv.Value);
                }
                sb.AppendJoin(", ", keys);
                var cmd = sqliteConnection.CreateCommand();
                cmd.CommandText = sb.ToString();

                return cmd;
            }
            default:
                throw new Exception("Unknown operation type");
        }
    }



}