namespace Juke.Web.Core.Routing;

public class StaticRouteNode: RouteNode {
    public StaticRouteNode(string pathPart) {
        PathPart = pathPart;
    }
    public string PathPart { get; init; }
}