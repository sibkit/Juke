using System.Collections;
using System.Data;
using System.Numerics;
using Juke.Accessing;
using Juke.Mapping;
using Juke.Querying;
using AdoSqlite = Microsoft.Data.Sqlite;

namespace Juke.Sqlite;



public class SqliteConnection: IConnection {

    private readonly AdoSqlite.SqliteConnection _adoConnection;
    private SqliteTransaction? _currentTransaction;
    private readonly SqliteDriver _driver;
    
    internal SqliteConnection(SqliteDriver driver, AdoSqlite.SqliteConnection adoConnection) {
        _driver = driver;
        _adoConnection = adoConnection;
        _adoConnection.Open();
    }
    
    public void Close() {
        _adoConnection.Close();
    }
    public bool IsOpen() {
        return _adoConnection.State == ConnectionState.Open;
    }
    public ITransaction BeginTransaction() {
        var adoTx = _adoConnection.BeginTransaction();
        _currentTransaction = new SqliteTransaction(adoTx, TransactionState.Opened);
        return _currentTransaction;
    }

    public ITransaction? CurrentTransaction => _currentTransaction;

    public void ExecuteOperation(IEntityOperation operation) {
        BeforeOperationExecute?.Invoke(this, EventArgs.Empty);
        var command = _driver.SqlBuilder.BuildOperationCommand(operation, _adoConnection);
        command.ExecuteNonQuery();
        AfterOperationExecute?.Invoke(this, EventArgs.Empty);
    }
    public event EventHandler? AfterOperationExecute;
    public event EventHandler? BeforeOperationExecute;
    
    public ISequence<T> GetSequence<T>(SequenceMap map) 
        where T : struct, INumber<T> {
        return _driver.GetSequence<T>(map);
        
    }

    private static IEnumerable<object?[]> Convert(IEnumerable source) {
        foreach (var item in source) {
            yield return (object?[])item;
        }
    }
    public IEnumerable<object?[]> GetReader(Query query) {
        var command = _adoConnection.CreateCommand();
        //command.CommandText = 
        return Convert(command.ExecuteReader());
    }
}