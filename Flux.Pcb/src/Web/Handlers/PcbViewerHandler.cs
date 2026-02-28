/* PcbViewerHandler.cs */
using Flux.Pcb.Web.Components;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;

namespace Flux.Pcb.Web.Handlers;

public class PcbViewerHandler : PageHandler
{
    protected override ValueTask<IPage> CreatePageAsync(IHttpContext context)
    {
        var orderIdStr = context.Request.RouteValues["orderId"]?.ToString();
        if (!Guid.TryParse(orderIdStr, out var orderId)) throw new ArgumentException("Invalid Order ID");

        var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Orders", orderId.ToString());
        var layers = new List<string>();
        var failedLayers = new List<string>(); 
        var ignoredFiles = new List<string>(); // Для пропущенных

        if (Directory.Exists(dirPath))
        {
            layers = Directory.GetFiles(dirPath, "*.svg")
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrEmpty(name))
                .Cast<string>()
                .ToList();
                              
            var failedPath = Path.Combine(dirPath, "failed_layers.txt");
            if (File.Exists(failedPath)) {
                failedLayers = File.ReadAllLines(failedPath).ToList();
            }
            
            var ignoredPath = Path.Combine(dirPath, "ignored_files.txt");
            if (File.Exists(ignoredPath)) {
                ignoredFiles = File.ReadAllLines(ignoredPath).ToList();
            }
        }

        var viewerComponent = new PcbViewer
        {
            OrderId = orderId,
            Layers = layers,
            FailedLayers = failedLayers,
            IgnoredFiles = ignoredFiles // Передаем в компонент
        };

        var layoutFactory = context.RequestServices.Get<ILayoutFactory>();
        var breadcrumbs = new List<BreadcrumbItem> {
            new BreadcrumbItem { Label = "Главная", Url = "/" },
            new BreadcrumbItem { Label = "Заказы PCB", Url = "/pcb/new" },
            new BreadcrumbItem { Label = $"Заказ {orderId.ToString()[..8]}" }
        };

        var page = layoutFactory.CreateMainLayout(context, $"PCB {orderId.ToString()[..8]}", viewerComponent, breadcrumbs);
        return new ValueTask<IPage>(page);
    }
}