
using Juke.Erp.WebHost.Components;
using Juke.Web.Components.Fluid;
using Juke.Web.Core;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Render;

namespace Juke.Erp.WebHost.Handlers;

public class HomeHandler : PageHandler 
{
    protected override async ValueTask<IPage> CreatePageAsync(IHttpContext context) 
    {
        // 1. Загрузка данных (В будущем здесь будет await dbContext...)
        // Можно загружать параллельно!
        var userTask = Task.FromResult("Alex (CEO)");
        var salesTask = Task.FromResult(45200.50m);
        
        await Task.WhenAll(userTask, salesTask);

        // 2. Сборка UI
        var page = new MasterPage 
        {
            Title = "Sibtronic ERP - Executive Dashboard",
            Header = new Header 
            { 
                CurrentUser = userTask.Result,
                CurrentTheme = "Dark" 
            },
            Sidebar = new SidebarComponent(),
            Footer = new FooterComponent(),
            Breadcrumbs = new BreadcrumbComponent(),
            MainContent = new DashboardContent
            {
                SalesToday = salesTask.Result,
                ActiveUsers = 312,
                SystemAlerts = 2
            }
        };

        return page;
    }
}