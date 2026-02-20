namespace Juke.Web.Core.Routing;


public class StaticRouteNode: RouteNodeBase {
    public StaticRouteNode(string pathPart) {
        PathPart = pathPart;
    }
    public string PathPart { get; init; }
}

public interface IPathPartMatcher {
    bool IsMatch(string pathPart);
}

public class DynamicRouteNode: RouteNodeBase {
    
    public string ParameterName { get; }
    public IPathPartMatcher Matcher { get; }
    
    public DynamicRouteNode(IPathPartMatcher matcher, string parameterName) {
        Matcher = matcher;
        ParameterName = parameterName;
    }
    
    public bool IsMatch(string pathPart) {
        return Matcher.IsMatch(pathPart);
    }
}

public class GroupRouteNode : RouteNodeBase {
    public string? PathPart { get; private set; }
    
    internal void SetMountPath(string pathPart) {
        if (PathPart != null) {
            throw new InvalidOperationException($"Эта группа уже смонтирована по пути '{PathPart}'.");
        }
        PathPart = pathPart;
    }
}