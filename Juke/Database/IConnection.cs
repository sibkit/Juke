using Juke.Database.Operation;
using Juke.Database.Transaction;
using Juke.Mapping;
using Juke.Querying;

namespace Juke.Database;

public interface IConnection {
    void Close();
    bool IsOpen();
    ITransaction BeginTransaction();
    ITransaction CurrentTransaction { get; }
    void ExecuteOperation(EntityOperation operation);
    event EventHandler AfterOperationExecute;
    event EventHandler BeforeOperationExecute;
    ISequence<T> GetSequence<T>(SequenceMap map);
    IDatabaseIterator Iterate(Query query);
}