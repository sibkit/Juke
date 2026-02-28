using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;

namespace Juke.Web.Core.Routing;

public interface IRouteNode {
    IReadOnlyList<IRouteNode> ChildNodes { get; }
    IHandler? GetHandler(Method method);
}

public abstract class RouteNode : IRouteNode {
    private static readonly int _methodsCount = Enum.GetValues<Method>().Count(val => val >= 0);
    private readonly List<IRouteNode> _childNodes = [];
    
    public IReadOnlyList<IRouteNode> ChildNodes => _childNodes;
    private readonly IHandler?[] _handlers = new IHandler?[_methodsCount];
    
    public IRouteNode AddNode(IRouteNode node) {
        if (node is GroupRouteNode) {
            throw new InvalidOperationException("GroupRouteNode must be added with method Mount(pathPart, group).");
        }
        _childNodes.Add(node);
        return node;
    }
    
    public GroupRouteNode Mount(string pathPart, GroupRouteNode group) {
        group.SetMountPath(pathPart);
        _childNodes.Add(group);
        return group;
    }
    
    // Существующий метод (добавление прямо в текущий узел)
    public void AddHandler(Method method, IHandler handler) {
        if ((int)method < 0) { 
            throw new ArgumentOutOfRangeException(nameof(method), "Cannot add handler for undefined or negative methods.");
        }
        _handlers[(int)method] = handler;
    }

    // НОВЫЙ МЕТОД: Автоматический билдер дерева роутов из строки
    public void AddHandler(Method method, IHandler handler, string path) {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        RouteNode currentNode = this;

        foreach (var segment in segments) {
            RouteNode? nextNode = null;
            bool isDynamic = segment.StartsWith('{') && segment.EndsWith('}');
            string paramName = isDynamic ? segment[1..^1] : segment;

            // Ищем, нет ли уже такого узла на текущем уровне
            foreach (var child in currentNode.ChildNodes) {
                if (isDynamic && child is DynamicRouteNode dyn && dyn.ParameterName == paramName) {
                    nextNode = dyn;
                    break;
                } else if (!isDynamic && child is StaticRouteNode stat && stat.PathPart.Equals(segment, StringComparison.OrdinalIgnoreCase)) {
                    nextNode = stat;
                    break;
                }
            }

            // Если узла нет — создаем новый
            if (nextNode == null) {
                nextNode = isDynamic 
                    ? new DynamicRouteNode(new StringMatcher(), paramName) 
                    : new StaticRouteNode(segment);
                    
                currentNode.AddNode(nextNode);
            }
            
            currentNode = nextNode;
        }

        // Вешаем обработчик на самый последний (листовой) узел
        currentNode.AddHandler(method, handler);
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