// using Juke.Mapping;
//
// namespace Juke;
//
// public class EntityBuilder<T>
//     where T : class, new(){
//     private readonly EntityMapper<T> _mapper;
//
//     public EntityBuilder(EntityMapper<T> mapper) {
//         _mapper = mapper;
//     }
//
//     public T Build(object?[] resultValues) {
//         var content = _mapper.BuildContent(resultValues);
//         return Build(content);
//     }
//
//     public T Build(EntityContent content) {
//         var result = new T();
//         try {
//             foreach (var fm in _mapper.Map.FieldMaps) {
//                 _mapper.WriteValue(result, fm, content.GetFieldValue(fm.Index));
//             }
//         } catch (Exception ex) {
//             throw new Exception($"Invalid mapping for: {_mapper.Map.EntityName} entity.", ex);
//         }
//         return result;
//     }
// }