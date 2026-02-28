using Juke.Web.Core.Routing;

namespace Juke.Web.Core;

public interface IJukeWebModule
{
    string Name { get; }
    
    GroupRouteNode GetModuleRoute(); 
}