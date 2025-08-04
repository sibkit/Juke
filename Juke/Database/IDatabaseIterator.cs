using Juke.Querying;

namespace Juke.Database;

public interface IDatabaseIterator: IEnumerator<object?[]> {
    Query Query { get; }
}