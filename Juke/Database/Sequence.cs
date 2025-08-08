using System.Numerics;

namespace Juke.Database;

public interface ISequence<out T>
    where T: struct, INumber<T>{
    string Name { get;}
    T NextValue();
    T CurrentValue();
}