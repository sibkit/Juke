using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;

namespace Juke.Web.Core.Routing;

public class Router {
    public Dictionary<int, IErrorHandler> ErrorHandlers { get; } = [];
    public GroupRouteNode RootNode { get; }
    
    public Router(GroupRouteNode rootNode) {
        RootNode = rootNode;
    }
    
    public IHandler? Resolve(IHttpContext context) {
        
        var method = context.Request.Method;
        if (method == Method.UNDEFINED) {
            context.Response.StatusCode = 400;
            return ErrorHandlers.GetValueOrDefault(400);
        }

        var path = context.Request.Path.AsSpan().Trim('/');
        
        if (path.IsEmpty) {
            var rootHandler = RootNode.GetHandler(method);
            if(rootHandler == null) {
                context.Response.StatusCode = 404;
                rootHandler = ErrorHandlers.GetValueOrDefault(404);
            }
            return rootHandler;
        }
        
        var handler = MatchRecursive(RootNode.ChildNodes, path, method, context.Request.RouteValues);
        
        if (handler != null) {
            return handler; 
        }
        
        context.Response.StatusCode = 404;
        return ErrorHandlers.GetValueOrDefault(404);
    }

private IHandler? MatchRecursive(
        IReadOnlyList<IRouteNode> nodes,
        ReadOnlySpan<char> path,
        Method method,
        Dictionary<string, object> routeValues) {

        var slashIndex = path.IndexOf('/');
        
        ReadOnlySpan<char> currentSegment;
        ReadOnlySpan<char> remainingPath;

        if (slashIndex == -1) {
            currentSegment = path;
            remainingPath = default;
        } else {
            currentSegment = path[..slashIndex];
            remainingPath = path[(slashIndex + 1)..].TrimStart('/');
        }

        var isLastSegment = remainingPath.IsEmpty;

        foreach (var node in nodes) {
            var isMatch = false;
            (string key, object? value)? dynamicEntry = null;

            switch (node) {
                case StaticRouteNode staticNode: {
                    if (currentSegment.Equals(staticNode.PathPart, StringComparison.OrdinalIgnoreCase)) {
                        isMatch = true;
                    }
                    break;
                }
                case GroupRouteNode groupNode: {
                    if (groupNode.PathPart != null && currentSegment.Equals(groupNode.PathPart, StringComparison.OrdinalIgnoreCase)) {
                        isMatch = true;
                    }
                    break;
                }
                case DynamicRouteNode dynamicNode: {
                    if (dynamicNode.TryMatch(currentSegment, out var parsedValue)) {
                        isMatch = true;
                        dynamicEntry = (dynamicNode.ParameterName, parsedValue);
                    }
                    break;
                }
            }

            if (isMatch) {
                if (isLastSegment) {
                    var handler = node.GetHandler(method);
                    if (handler != null) {
                        if (dynamicEntry != null) {
                            routeValues[dynamicEntry.Value.key] = dynamicEntry.Value.value;
                        }
                        return handler;
                    }
                } else {
                    var handler = MatchRecursive(node.ChildNodes, remainingPath, method, routeValues);

                    if (handler != null) {
                        if (dynamicEntry != null) {
                            routeValues[dynamicEntry.Value.key] = dynamicEntry.Value.value;
                        }
                        return handler;
                    }
                }
            }
        }
        return null;
    }
}