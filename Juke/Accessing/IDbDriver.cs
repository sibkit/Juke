namespace Juke.Accessing;

public interface IDbDriver {
    MappingData MappingData { get; init; }
    IConnection createConnection();
}