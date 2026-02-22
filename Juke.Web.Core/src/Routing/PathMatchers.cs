namespace Juke.Web.Core.Routing;

public interface IPathPartMatcher {
    bool TryMatch(ReadOnlySpan<char> pathPart, out object? parsedValue);
}

public class IntMatcher : IPathPartMatcher {
    public bool TryMatch(ReadOnlySpan<char> pathPart, out object? parsedValue) {

        if (int.TryParse(pathPart, out var result)) {
            parsedValue = result; 
            return true;
        }
        
        parsedValue = null;
        return false;
    }
}

public class StringMatcher : IPathPartMatcher {
    
    public bool TryMatch(ReadOnlySpan<char> pathPart, out object? parsedValue) {
        if (pathPart.IsEmpty) {
            parsedValue = null;
            return false;
        }
        
        parsedValue = pathPart.ToString(); 
        return true;
    }
}