using Juke.Querying;

namespace Juke.Accessing;

public interface IDbEnumerable: IEnumerable<object?[]> {
    Query Query { get; }
}