using System.Collections;
using System.Numerics;
using System.Text;
using Juke.Accessing;
using Juke.Mapping;
using AdoSqlite = Microsoft.Data.Sqlite;

namespace Juke.Sqlite;

public interface ISqliteSequence {
    public string Name { get; }
    AdoSqlite.SqliteCommand GetInsertCommand();
    AdoSqlite.SqliteCommand GetUpdateCommand();
}

public class SqliteSequence<T>: ISequence<T>, ISqliteSequence
    where T: struct, INumber<T>
{
    private readonly AdoSqlite.SqliteConnection _sequenceConnection;
    private readonly SequenceMap _sequenceMap;
    private readonly SequencesTableInfo  _sequencesTableInfo;
    
    private T? _currentValue;
    
    public SqliteSequence(SequenceMap sequenceMap, AdoSqlite.SqliteConnection sequenceConnection, SequencesTableInfo sequencesTableInfo) {
        _sequenceMap = sequenceMap;
        _sequenceConnection = sequenceConnection;
        _sequencesTableInfo = sequencesTableInfo;
    }

    public string Name => _sequenceMap.SequenceName;
    
    private T ReadFromDb() {
        var command = _sequenceConnection.CreateCommand();
        command.CommandText = "SELECT " + _sequencesTableInfo.ValueColumn + " FROM " + _sequencesTableInfo.TableName + " WHERE " + _sequencesTableInfo.NameColumn + " = '" + _sequenceMap.DbSequenceName +"'";
        var reader = command.ExecuteReader();
        var rows = new ArrayList(1);
        while (reader.Read()) {
            rows.Add(reader.GetValue(0));
        }
        switch (rows.Count) {
            case 1:
                return (T)(rows[0] ?? throw new Exception("Sequence read error 1"));
            case 0:
                InsertIntoDb(default(T));
                return default(T);
            default:
                throw new Exception("Sequence read error 2");
        }
    }

    private AdoSqlite.SqliteCommand? _insertCommand;

    public AdoSqlite.SqliteCommand GetInsertCommand() {
        if (_insertCommand == null) {
            _insertCommand = _sequenceConnection.CreateCommand();
            var sb = new StringBuilder("INSERT INTO ");
            sb.Append(_sequencesTableInfo.TableName);
            sb.Append(" ( ");
            sb.Append(_sequencesTableInfo.NameColumn);
            sb.Append(", ");
            sb.Append(_sequencesTableInfo.ValueColumn);
            sb.Append(") VALUES ('");
            sb.Append(_sequenceMap.DbSequenceName);
            sb.Append("', @s)");
            _insertCommand.CommandText = sb.ToString();
        } else
            _insertCommand.Parameters.Clear();
        return _insertCommand;
    }
    
    private void InsertIntoDb(T? value) {
        var command = GetInsertCommand();
        command.Parameters.AddWithValue("@s", value);
        var result = command.ExecuteNonQuery();
        if(result != 1)
            throw new Exception("Sequence insert error");
    }

    private AdoSqlite.SqliteCommand? _updateCommand;

    public AdoSqlite.SqliteCommand GetUpdateCommand() {
        if (_updateCommand == null) {
            _updateCommand = _sequenceConnection.CreateCommand();
            var sb = new StringBuilder("UPDATE ");
            sb.Append(_sequencesTableInfo.TableName);
            sb.Append(" SET ");
            sb.Append(_sequencesTableInfo.ValueColumn);
            sb.Append(" = @s WHERE ");
            sb.Append(_sequencesTableInfo.NameColumn);
            sb.Append(" = '");
            sb.Append(_sequenceMap.DbSequenceName);
            sb.Append('\'');
            _updateCommand.CommandText = sb.ToString();
        } else
            _updateCommand.Parameters.Clear();
        return _updateCommand;
    }

    private void UpdateInDb(T value) {
        var updateCommand = GetUpdateCommand();
        updateCommand.Parameters.AddWithValue("@s", value);
        var rows = updateCommand.ExecuteNonQuery();
        if(rows != 1)
            throw new Exception("Sequence update error");
    }

    public T NextValue() {
        _currentValue ??= ReadFromDb();
        _currentValue++;
        UpdateInDb(_currentValue.Value);
        return _currentValue.Value;
    }

    public T CurrentValue() {
        _currentValue ??= ReadFromDb();
        return _currentValue.Value;
    }
}
