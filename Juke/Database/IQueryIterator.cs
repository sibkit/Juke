using Juke.Querying;

namespace Juke.Database;

public interface IQueryIterator: IEnumerator<object?[]> {
    Query Query { get; }
}