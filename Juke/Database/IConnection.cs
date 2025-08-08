using System.Data;
using System.Numerics;
using Juke.Mapping;
using Juke.Querying;

namespace Juke.Database;

public interface IConnection {
    void Close();
    bool IsOpen();
    ITransaction BeginTransaction();

    ITransaction? CurrentTransaction { get; }
    void ExecuteOperation(IEntityOperation operation);
    event EventHandler AfterOperationExecute;
    event EventHandler BeforeOperationExecute;
    ISequence<T> GetSequence<T>(SequenceMap map) 
        where T : struct, INumber<T>;
    IQueryIterator Iterate(Query query);
}