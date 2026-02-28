/* FluxPcbModule.cs */
using Flux.Pcb.Web.Handlers;
using Juke.Web.Core;
using Juke.Web.Core.Routing;

namespace Flux.Pcb;

public class FluxPcbModule : IJukeWebModule
{
    public string Name => "Flux.Pcb";
    
    public GroupRouteNode GetModuleRoute()
    {
        var moduleRoot = new GroupRouteNode();
        moduleRoot.AddHandler(Juke.Web.Core.Http.Method.GET, new PcbNewOrderHandler(), "new");
        moduleRoot.AddHandler(Juke.Web.Core.Http.Method.POST, new PcbUploadHandler(), "upload");
        moduleRoot.AddHandler(Juke.Web.Core.Http.Method.GET, new PcbLayerAssetHandler(), "api/order/{orderId}/layer/{fileName}");
        
        moduleRoot.AddHandler(Juke.Web.Core.Http.Method.GET, new PcbRawFileHandler(), "api/order/{orderId}/raw/{fileName}");
        
        moduleRoot.AddHandler(Juke.Web.Core.Http.Method.GET, new PcbViewerHandler(), "viewer/{orderId}");
        return moduleRoot;
    }
}