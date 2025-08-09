// using System.Collections;
// using Juke.Accessing;
// using Juke.Mapping;
// using Juke.Querying;
//
// namespace Juke;
//
// public class EntityIterator<T>: IEnumerator<T>
// where T : class, new() {
//     private readonly IDbIterator _dbIterator;
//     private readonly EntityBuilder<T> _entityBuilder;
//     
//     public EntityIterator(MappingData mappingData, IDbIterator dbIterator) {
//         _dbIterator = dbIterator;
//
//         if (dbIterator.Query is EntityQuery eq) {
//             var mapper = mappingData.GetMapper(eq.EntityName);
//             _entityBuilder = new EntityBuilder<T>(mapper as EntityMapper<T> ?? throw new Exception("Mapper type does not correspond with EntityIterator"));
//         } else {
//             throw new Exception("EntityIterator only supports EntityQuery");
//         }
//         
//     }
//     
//     public bool MoveNext() {
//         return _dbIterator.MoveNext();
//     }
//     public void Reset() {
//         _dbIterator.Reset();
//     }
//
//     T IEnumerator<T>.Current {
//         get {
//             var dbc = _dbIterator.Current;
//             return (dbc == null ? null : _entityBuilder.Build(dbc)) ?? throw new InvalidOperationException();
//         }
//     }
//
//     object? IEnumerator.Current => (this as IEnumerator<T>).Current;
//
//     public void Dispose() {
//         _dbIterator.Dispose();
//     }
// }