using Juke.Database;
using AdoSqlite = Microsoft.Data.Sqlite;
namespace Juke.Sqlite;

public class SqliteTransaction: ITransaction {
    private readonly AdoSqlite.SqliteTransaction _adoTransaction;
    private TransactionState _state;
    
    public SqliteTransaction(AdoSqlite.SqliteTransaction adoTransaction, TransactionState state) {
        _adoTransaction = adoTransaction;
        _state = state;
    }
    
    public void Commit() {
        _adoTransaction.Commit();
        _state = TransactionState.Committed;
    }
    public void Rollback() {
        _adoTransaction.Rollback();
        _state = TransactionState.Aborted;
    }

    public TransactionState State {
        get => _state;
        private set {
            var oldState = _state;
            _state = value;
            StateChanged?.Invoke(this, (oldState, _state));
        }
    }

    public event EventHandler<(TransactionState oldState, TransactionState newState)>? StateChanged;
}