namespace Juke.Accessing;

public enum TransactionState {
    Opened,
    Committed,
    Aborted,
    Closed
}
public interface ITransaction {
    void Commit();
    void Rollback();
    TransactionState State { get; }

    // IEntityOperation[] GetOperations();
    event EventHandler<(TransactionState oldState, TransactionState newState)> StateChanged; 
}