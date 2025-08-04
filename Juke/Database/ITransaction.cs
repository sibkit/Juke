namespace Juke.Database.Transaction;

public enum TransactionState {
    Opened,
    Committed,
    Aborted,
    Closed
}
public interface ITransaction {
    void Begin();
    void Commit();
    void Rollback();
    TransactionState State { get; }
}