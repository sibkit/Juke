/* Juke.Web.Core/Handlers/AssetHandler.cs */
using System.Threading.Tasks;
using Juke.Web.Core.Assets;
using Juke.Web.Core.Http;
using StringContent = Juke.Web.Core.Assets.StringContent;

namespace Juke.Web.Core.Handlers;

public class AssetHandler : IRequestHandler 
{
    public async Task HandleAsync(IHttpContext context) 
    {
        var registry = context.RequestServices.Get<AssetRegistry>();
        var path = context.Request.Path.TrimStart('/'); 

        if (registry.TryGet(path, out var resource) && resource != null) 
        {
            context.Response.StatusCode = 200;
            context.Response.AddHeader("Cache-Control", "public, max-age=31536000, immutable");

            if (resource.Content is StringContent str) {
                context.Response.SetContentType(str.Type == StringContentType.Css ? "text/css" : "application/javascript");
                await context.Response.WriteAsync(str.Text);
            } 
            else if (resource.Content is BinaryContent bin) {
                context.Response.SetContentType(bin.Type == BinaryContentType.Font ? "font/woff2" : "image/webp");
                await context.Response.Body.WriteAsync(bin.Data); // Zero-Allocation запись байтов!
            }
        } 
        else {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Not found.");
        }
    }
}