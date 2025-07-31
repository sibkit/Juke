namespace Juke.Mapping;

public class KeyValue {
    public required FieldMap FieldMap { get; init; }
    public object? Value { get; init; }

    public override bool Equals(object? obj) {
        if (obj is KeyValue kv) {
            return Equals(kv);
        }
        return false;
    }

    protected bool Equals(KeyValue other) {
        if (Value is null && other.Value is null)
            return true;
        if(!FieldMap.Equals(other.FieldMap))
            return false;
        return Value is not null && Value.Equals(other.Value);
    }

    public override int GetHashCode() {
        return HashCode.Combine(FieldMap, Value);
    }

    public override string ToString() {
        return FieldMap + ": " + Value;
    }
}