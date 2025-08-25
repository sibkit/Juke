using System.Numerics;

namespace Juke.Accessing;

public interface ISequence<out T>
    where T: struct, INumber<T>{
    string Name { get;}
    T NextValue();
    T CurrentValue();
}