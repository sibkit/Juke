/* Flux.Pcb/src/Web/Handlers/PcbLayerAssetHandler.cs */
using System.IO;
using System.Threading.Tasks;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;

namespace Flux.Pcb.Web.Handlers;

public class PcbLayerAssetHandler : IRequestHandler
{
    public async Task HandleAsync(IHttpContext context)
    {
        // Извлекаем параметры из роута (мы их настроим в Program.cs)
        var orderId = context.Request.RouteValues["orderId"]?.ToString();
        var fileName = context.Request.RouteValues["fileName"]?.ToString();

        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(fileName))
        {
            context.Response.StatusCode = 404;
            return;
        }

        // Защита от Directory Traversal (чтобы не передали "../../etc/passwd")
        fileName = Path.GetFileName(fileName); 

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Orders", orderId, fileName);

        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            return;
        }

        context.Response.StatusCode = 200;
        context.Response.SetContentType("image/svg+xml");
        
        // Zero-allocation стриминг файла напрямую в Response
        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await fs.CopyToAsync(context.Response.Body);
    }
}