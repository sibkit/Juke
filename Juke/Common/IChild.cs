using Juke.Querying;

namespace Juke.Common;

public interface IChild<T> {
    QueryElement? Parent { get; set; }
}