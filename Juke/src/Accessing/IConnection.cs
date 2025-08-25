using System.Numerics;
using Juke.Mapping;
using Juke.Querying;

namespace Juke.Accessing;

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
    IEnumerable<object?[]> GetReader(Query query);
}