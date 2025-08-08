using Juke.Querying;

namespace Juke.Accessing;

public interface IQueryIterator: IEnumerator<object?[]> {
    Query Query { get; }
}