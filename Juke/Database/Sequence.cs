namespace Juke.Database;

public interface ISequence<out T> {
    string Name { get; }
    T NextValue();
    T CurrentValue();
}

public enum SequenceOperationType
{
    NextValue, CurrentValue
}

public class SequenceOperation {
    
}