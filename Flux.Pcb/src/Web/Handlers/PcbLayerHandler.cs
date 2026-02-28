using System;
using System.IO;
using System.Threading.Tasks;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;

namespace Flux.Pcb.Web.Handlers;

public class PcbLayerHandler : IRequestHandler
{
    public async Task HandleAsync(IHttpContext context)
    {
        var orderIdStr = context.Request.RouteValues["orderId"]?.ToString();
        var fileName = context.Request.RouteValues["fileName"]?.ToString();

        if (string.IsNullOrEmpty(orderIdStr) || string.IsNullOrEmpty(fileName))
        {
            context.Response.StatusCode = 400;
            return;
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Orders", orderIdStr, fileName);

        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            return;
        }

        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var ct = ext switch
        {
            ".svg" => "image/svg+xml",
            ".txt" or ".csv" or ".drl" => "text/plain",
            _ => "application/octet-stream"
        };
        context.Response.SetContentType(ct);

        // ПРЯМОЕ ПОТОКОВОЕ ЧТЕНИЕ: Файл читается с диска и сразу уходит в сеть. ОЗУ не расходуется!
        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
        await fs.CopyToAsync(context.Response.Body);
    }
}