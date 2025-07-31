namespace Juke.Database.Transaction;

public enum TransactionState {
    Opened,
    Committed,
    Aborted,
    Closed
}