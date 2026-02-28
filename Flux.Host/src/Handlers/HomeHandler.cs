/* Flux.Host/Handlers/HomeHandler.cs */
using System.Collections.Generic;
using System.Threading.Tasks;
using Flux.Host.Components;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;

namespace Flux.Host.Handlers;

public class HomeHandler : PageHandler
{
    protected override ValueTask<IPage> CreatePageAsync(IHttpContext context)
    {
        // 1. Здесь в будущем будет обращение к EF Core для получения реальных данных
        // var dbContext = context.RequestServices.Get<ErpDbContext>();
        // var salesToday = await dbContext.Orders.Where(x => x.Date == DateTime.Today).SumAsync(x => x.Total);
        
        var mockedSalesToday = 15400.50m;

        // 2. Создаем ТОЛЬКО контент главной страницы
        var dashboardContent = new DashboardContent 
        {
            StoreId = "main-branch",
            StoreName = "Central Store",
            SalesToday = mockedSalesToday
        };

        // 3. Запрашиваем фабрику лэйаутов
        var layoutFactory = context.RequestServices.Get<ILayoutFactory>();

        // 4. Формируем хлебные крошки для текущей страницы
        var breadcrumbs = new List<BreadcrumbItem> 
        {
            new BreadcrumbItem { Label = "Главная" }
        };

        // 5. Делегируем сборку страницы Хосту
        var page = layoutFactory.CreateMainLayout(
            context, 
            title: "Dashboard", 
            mainContent: dashboardContent, 
            breadcrumbs: breadcrumbs
        );

        return new ValueTask<IPage>(page);
    }
}