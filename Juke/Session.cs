using Juke.Accessing;
using Juke.Mapping;
using Juke.Querying;

namespace Juke;

public class Session {
    public Session(IConnection connection, Database database) {
        Connection = connection;
        Database = database;
    }
    
    public IConnection Connection { get; }
    public Database Database { get; }

    private IEnumerable<T> Convert<T>(IEnumerable<object?[]> source)
        where T : class, new() {
        if (Database.Driver.MappingData.GetMapper(typeof(T)) is not EntityMapper<T> mapper) {
            throw new Exception("No mapper found for type " + typeof(T).Name);
        }

        foreach (var item in source) {
            var content = mapper.BuildContent(item);
            var entity = new T();
            mapper.BindToEntity(content, entity);
            yield return entity;

        }
    }

    public IEnumerable<T> GetQueryReader<T>(Query query) where T : class, new() {
        var reader = Connection.GetReader(query);
        return Convert<T>(reader);
    }

    public IEnumerable<object?[]> GetQueryReader(Query query) {
        return Connection.GetReader(query);
    }
}