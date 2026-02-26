/* Juke.Erp.WebHost/Handlers/HomeHandler.cs */
using System.Threading.Tasks;
using Juke.Web.Core;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Render;
using Juke.Erp.WebHost.Components;
using Juke.Erp.WebHost.Pages;
using Juke.Web.Components.Fluid;
using Juke.Web.Core.Http;

namespace Juke.Erp.WebHost.Handlers;

public class HomeHandler : PageHandler
{
    protected override ValueTask<IPage> CreatePageAsync(IHttpContext context)
    {
        // 1. Здесь в будущем будет обращение к EF Core для получения реальных данных
        // var dbContext = context.RequestServices.Get<ErpDbContext>();
        // var salesToday = await dbContext.Orders.Where(x => x.Date == DateTime.Today).SumAsync(x => x.Total);
        
        var mockedSalesToday = 15400.50m;

        // 2. Собираем дерево компонентов (Zero-Allocation рендеринг и сборка активов начнутся позже)
        var page = new MasterPage
        {
            Title = "Dashboard - Juke ERP",
            Language = "en",
            
            Header = new Header 
            { 
                CurrentUser = "Admin", 
                CurrentTheme = "Light" 
            },
            
            MainContent = new DashboardContent 
            {
                StoreId = "main-branch",
                StoreName = "Central Store",
                SalesToday = mockedSalesToday
            },
            
             Sidebar = new SidebarComponent(),
             Footer = new FooterComponent()
        };

        // 3. Возвращаем ValueTask (избегаем выделения памяти под Task, так как операция синхронная)
        return new ValueTask<IPage>(page);
    }
}