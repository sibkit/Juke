// using Juke.Ado.Net.Sql;
// using Juke.Database;
//
// namespace Juke.Ado.Net;
//
// public class AdoDriver: IDatabaseDriver {
//     
//     public required string ConnectionString { get; init; }
//     public required MappingData MappingData { get; init; }
//     //public required ISqlBuilder SqlBuilder { get; init; }
//     
//     public IConnection createConnection() {
//         var connection = new AdoConnection(this);
//         return connection;
//     }
// }