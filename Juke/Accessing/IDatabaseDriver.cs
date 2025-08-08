namespace Juke.Accessing;

public interface IDatabaseDriver {
    MappingData MappingData { get; init; }
    IConnection createConnection();
}