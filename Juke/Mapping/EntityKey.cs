using System.Transactions;

namespace Juke.Mapping;

public class EntityKey {
    public required EntityMap EntityMap { get; init; }
    public required KeyValue[] Values { get; init; }

    public override bool Equals(object? obj) {
        if (object.Equals(this, obj))
            return true;
        if (obj is EntityKey ek) {
            return Equals(ek);
        }
        return false;
    }

    protected bool Equals(EntityKey other) {
        if(Values.Length != other.Values.Length)
            return false;
        for (var i = 0; i < Values.Length; i++) {
            if(!Values[i].Equals(other.Values[i]))
                return false;
        }
        return true;
    }

    public override int GetHashCode() {
        return Values.GetHashCode();
    }

    public override string ToString() {
        var result = Values.Aggregate("EntityKey [", (current, value) => current + (value + ", "));
        result = result[..^2];
        result += "]";
        return result;
    }
}