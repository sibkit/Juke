using System.Numerics;
using Juke.Accessing;
using Juke.Mapping;
using AdoSqlite = Microsoft.Data.Sqlite;

namespace Juke.Sqlite;

public class SqliteDriver: IDbDriver {
    private AdoSqlite.SqliteConnection? _sequenceConnection;
    
    public required MappingData MappingData { get; init; }
    public SqlBuilder SqlBuilder { get; init; } = new SqlBuilder();

    public required string ConnectionString { get; init; }
    public SequencesTableInfo? SequencesTableInfo { get; init; }
    
    public AdoSqlite.SqliteConnection SequenceConnection {
        get {
            if (_sequenceConnection == null) {
                _sequenceConnection = new AdoSqlite.SqliteConnection(ConnectionString);
                _sequenceConnection.Open();
            }
            return _sequenceConnection;
        }
    }
    
    public IConnection createConnection() {
        var connection = new SqliteConnection(this, new AdoSqlite.SqliteConnection(ConnectionString));
        return connection;
    }

    private readonly Dictionary<string, ISqliteSequence> _secuencesMap = new();
    
    public ISequence<T> GetSequence<T>(SequenceMap map) where T : struct, INumber<T> {
        if (map.SequenceValueType != typeof(T))
            throw new Exception($"Sequence value type {map.DbSequenceName} does not match type {typeof(T)}");
        if(_secuencesMap.TryGetValue(map.DbSequenceName, out var value)) {
            return value as ISequence<T> ?? throw new InvalidOperationException();
        }
        if(SequencesTableInfo == null)
            throw new Exception($"Sequence info is null");
        value = new SqliteSequence<T>(map, _sequenceConnection ?? throw new Exception("SqlDriver: GetSequence"), SequencesTableInfo);
        _secuencesMap.Add(map.DbSequenceName, value);
        return value as ISequence<T> ?? throw new InvalidOperationException();
    }
}