namespace Juke.Common;

public interface IChild<T> {
    T? Parent { get; set; }
}