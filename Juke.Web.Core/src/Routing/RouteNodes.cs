namespace Juke.Web.Core.Routing;

public interface IRouteNode {
    IReadOnlyList<IRouteNode> ChildNodes { get; }
    IHandler? GetHandler(Method method);
}

public abstract class RouteNodeBase : IRouteNode {
    private static readonly int _methodsCount = Enum.GetValues<Method>().Count(val => val >= 0);
    private readonly List<IRouteNode> _childNodes = [];
    
    public IReadOnlyList<IRouteNode> ChildNodes => _childNodes;
    private readonly IHandler?[] _handlers = new IHandler?[_methodsCount];
    
    
    
    public IRouteNode AddNode(IRouteNode node) {
        if (node is GroupRouteNode) {
            throw new InvalidOperationException(
                "GroupRouteNode must be added with method Mount(pathPart, group)."
            );
        }
        _childNodes.Add(node);
        return node;
    }
    
    public GroupRouteNode Mount(string pathPart, GroupRouteNode group) {
        group.SetMountPath(pathPart);
        _childNodes.Add(group);
        return group;
    }
    
    public void AddHandler(Method method, IHandler handler) {
        if ((int)method < 0) { 
            throw new ArgumentOutOfRangeException(nameof(method), "Cannot add handler for undefined or negative methods.");
        }
        _handlers[(int)method] = handler;
    }

    public IHandler? GetHandler(Method method) {
        return _handlers[(int)method];
    }

    public IEnumerable<Method> SupportedMethods 
    {
        get {
            for (var i = 0; i < _handlers.Length; i++) {
                if (_handlers[i] != null) yield return (Method)i;
            }
        }
    }
}

public class StaticRouteNode: RouteNodeBase {
    public StaticRouteNode(string pathPart) {
        PathPart = pathPart;
    }
    public string PathPart { get; init; }
}

public class DynamicRouteNode: RouteNodeBase {
    
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

public class GroupRouteNode : RouteNodeBase {
    public string? PathPart { get; private set; }
    
    internal void SetMountPath(string pathPart) {
        if (PathPart != null) {
            throw new InvalidOperationException($"Эта группа уже смонтирована по пути '{PathPart}'.");
        }
        PathPart = pathPart;
    }
}