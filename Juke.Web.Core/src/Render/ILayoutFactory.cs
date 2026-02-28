using Juke.Web.Core.Http;

namespace Juke.Web.Core.Render;

public interface ILayoutFactory
{
    IPage CreateMainLayout(
        IHttpContext context, 
        string title, 
        IComponent mainContent, 
        List<BreadcrumbItem>? breadcrumbs = null);
}