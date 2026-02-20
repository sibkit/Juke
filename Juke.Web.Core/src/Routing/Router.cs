using System.Diagnostics;

namespace Juke.Web.Core.Routing;

public class Router {
    public Dictionary<int, IErrorHandler> ErrorHandlers { get; } = [];
    public GroupRouteNode RootNode { get; }
    
    public Router(GroupRouteNode rootNode) {
        RootNode = rootNode;
    }
    
    
    
    public IHandler? Resolve(IHttpContext context) {
        IHandler? handler;
        
        var method = context.Request.Method;
        if (method == Method.UNDEFINED) {
            context.Response.StatusCode = 400;
            return ErrorHandlers.GetValueOrDefault(400);
        }

        var path = context.Request.Path.Trim('/');
        
        if (string.IsNullOrEmpty(path)) {
            handler = RootNode.GetHandler(method);
            if(handler == null) {
                context.Response.StatusCode = 404;
                handler = ErrorHandlers.GetValueOrDefault(404);
            }
            return handler;
        }
        
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        handler = MatchRecursive(RootNode.ChildNodes, segments, 0, method, context.Request.RouteValues);
        
        if (handler != null) {
            return handler; 
        }
        
        context.Response.StatusCode = 404;
        return ErrorHandlers.GetValueOrDefault(404);
    }

    private IHandler? MatchRecursive(
        IReadOnlyList<IRouteNode> nodes,
        string[] segments,
        int segmentIndex,
        Method method,
        Dictionary<string, string> routeValues) {

        var currentSegment = segments[segmentIndex];
        var isLastSegment = segmentIndex == segments.Length - 1;

        foreach (var node in nodes) {
            var isMatch = false;
            (string key, string value)? dynamicEntry = null;
            // string? dynamicKey = null;
            // string? dynamicValue = null;

            switch (node) {
                case StaticRouteNode staticNode: {
                    if (staticNode.PathPart.Equals(currentSegment, StringComparison.OrdinalIgnoreCase)) {
                        isMatch = true;
                    }
                    break;
                }
                case DynamicRouteNode dynamicNode: {
                    if (dynamicNode.IsMatch(currentSegment)) {
                        isMatch = true;
                        dynamicEntry = (dynamicNode.ParameterName, currentSegment);
                    }
                    break;
                }
                case GroupRouteNode groupNode: { 
                    if (groupNode.PathPart != null && groupNode.PathPart.Equals(currentSegment, StringComparison.OrdinalIgnoreCase)) {
                        isMatch = true;
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
                    var handler = MatchRecursive(node.ChildNodes, segments, segmentIndex + 1, method, routeValues);

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