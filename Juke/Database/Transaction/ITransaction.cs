namespace Juke.Database.Transaction;

public interface ITransaction {
    void Begin();
    void Commit();
    void Rollback();
    TransactionState State { get; }
}