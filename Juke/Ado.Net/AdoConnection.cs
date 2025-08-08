// using System.Data;
// using Juke.Database;
// using Juke.Mapping;
// using Juke.Querying;
//
// namespace Juke.Ado.Net;
//
// public class AdoConnection: IConnection {
//
//     private IDbConnection _adoConnection;
//     
//     public AdoConnection(AdoDriver driver) {
//         
//     }
//     
//     public void Close() {
//         throw new NotImplementedException();
//     }
//     public bool IsOpen() {
//         throw new NotImplementedException();
//     }
//     public ITransaction BeginTransaction() {
//         throw new NotImplementedException();
//     }
//     public ITransaction CurrentTransaction { get; }
//     public void ExecuteOperation(IEntityOperation operation) {
//         throw new NotImplementedException();
//     }
//     public event EventHandler? AfterOperationExecute;
//     public event EventHandler? BeforeOperationExecute;
//     public ISequence<T> GetSequence<T>(SequenceMap map) {
//         throw new NotImplementedException();
//     }
//     public IQueryIterator Iterate(Query query) {
//         throw new NotImplementedException();
//     }
// }