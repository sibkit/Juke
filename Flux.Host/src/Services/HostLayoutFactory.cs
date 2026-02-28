using Flux.Host.Components;
using Flux.Host.Pages;
using Juke.Web.Components.Fluid;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;
using BreadcrumbItem = Juke.Web.Core.Render.BreadcrumbItem;

namespace Flux.Host.Services;

public class HostLayoutFactory : ILayoutFactory
{
    public IPage CreateMainLayout(IHttpContext context, string title, IComponent mainContent, List<BreadcrumbItem>? breadcrumbs = null)
    {
        var currentUser = "Admin"; 
        breadcrumbs ??= [ new BreadcrumbItem { Label = "Dashboard" } ];

        // Вычисляем активный раздел на основе URL
        var path = context.Request.Path;
        string activeMenu = "dashboard";
        
        if (path.StartsWith("/pcb", StringComparison.OrdinalIgnoreCase)) activeMenu = "pcb";
        else if (path.StartsWith("/sales", StringComparison.OrdinalIgnoreCase)) activeMenu = "sales";
        else if (path.StartsWith("/finance", StringComparison.OrdinalIgnoreCase)) activeMenu = "finance";

        return new MasterPage
        {
            Title = $"{title} - SIBTRONIC ERP",
            Language = "ru",
            Header = new Header { CurrentUser = currentUser, CurrentTheme = "Dark" },
            
            // Передаем активное меню
            Sidebar = new SidebarComponent { ActiveMenu = activeMenu },
            
            Breadcrumbs = new BreadcrumbComponent { Items = breadcrumbs },
            Footer = new FooterComponent(),
            MainContent = mainContent
        };
    }
}