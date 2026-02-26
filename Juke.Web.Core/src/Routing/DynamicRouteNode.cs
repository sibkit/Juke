namespace Juke.Web.Core.Routing;

public class DynamicRouteNode: RouteNode {
    
    public string ParameterName { get; }
    public IPathPartMatcher Matcher { get; }
    
    public DynamicRouteNode(IPathPartMatcher matcher, string parameterName) {
        Matcher = matcher;
        ParameterName = parameterName;
    }
    
    public bool TryMatch(ReadOnlySpan<char> pathPart, out object? parsedValue) {
        return Matcher.TryMatch(pathPart, out parsedValue);
    }
}