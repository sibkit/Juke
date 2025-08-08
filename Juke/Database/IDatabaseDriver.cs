namespace Juke.Database;

public interface IDatabaseDriver {
    MappingData MappingData { get; init; }
    IConnection createConnection();
}