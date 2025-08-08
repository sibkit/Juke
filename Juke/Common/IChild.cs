using Juke.Querying;

namespace Juke.Common;

public interface IChild<T>
where T: QueryElement{
    T? Parent { get; set;  }
}