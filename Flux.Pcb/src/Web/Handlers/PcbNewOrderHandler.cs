using Flux.Pcb.Web.Components;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;

namespace Flux.Pcb.Web.Handlers;

public class PcbNewOrderHandler : PageHandler
{
    protected override ValueTask<IPage> CreatePageAsync(IHttpContext context)
    {
        var form = new PcbUploadForm();
        var layoutFactory = context.RequestServices.Get<ILayoutFactory>();
        
        // Устанавливаем динамические крошки
        var breadcrumbs = new List<BreadcrumbItem> {
            new BreadcrumbItem { Label = "Главная", Url = "/" },
            new BreadcrumbItem { Label = "Заказы PCB", Url = "/pcb" },
            new BreadcrumbItem { Label = "Новый заказ" }
        };

        var page = layoutFactory.CreateMainLayout(context, "Новый заказ PCB", form, breadcrumbs);
        return new ValueTask<IPage>(page);
    }
}