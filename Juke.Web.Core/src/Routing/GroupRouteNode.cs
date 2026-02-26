namespace Juke.Web.Core.Routing;

public class GroupRouteNode : RouteNode {
    public string? PathPart { get; private set; }
    
    internal void SetMountPath(string pathPart) {
        if (PathPart != null) {
            throw new InvalidOperationException($"Эта группа уже смонтирована по пути '{PathPart}'.");
        }
        PathPart = pathPart;
    }
}