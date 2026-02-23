using System.Text;

namespace Juke.Erp.WebHost.Components;

using System.IO;
using System.Threading.Tasks;
using Juke.Web.Core;
using Juke.Web.Core.Render;
using Juke.Web.Fluid;



public class MasterPage : FluidComponent, IPage
{
    public string Title { get; set; } = "Sibtronic ERP";
    public string Language { get; set; } = "ru";

    // Строки для хранения собранных тегов
    private string _headResourcesHtml = string.Empty;
    private string _bodyResourcesHtml = string.Empty;

    // Регионы Layout'а
    private IComponent? _header;
    public IComponent? Header { get => _header; set { _header = value; if (value != null) AddChild(value); } }

    private IComponent? _sidebar;
    public IComponent? Sidebar { get => _sidebar; set { _sidebar = value; if (value != null) AddChild(value); } }

    private IComponent? _breadcrumbs;
    public IComponent? Breadcrumbs { get => _breadcrumbs; set { _breadcrumbs = value; if (value != null) AddChild(value); } }

    private IComponent? _footer;
    public IComponent? Footer { get => _footer; set { _footer = value; if (value != null) AddChild(value); } }

    private IComponent? _mainContent;
    public IComponent? MainContent { get => _mainContent; set { _mainContent = value; if (value != null) AddChild(value); } }

    // РЕАЛИЗАЦИЯ ИНЪЕКЦИИ РЕСУРСОВ
    public void InjectResources(IReadOnlyList<IWebResource> resources, IReadOnlyList<InlineScript> scripts)
    {
        var headBuilder = new StringBuilder();
        var bodyBuilder = new StringBuilder();
        var domReadyScripts = new StringBuilder();

        // 1. Внешние файлы (CSS/JS)
        foreach (var res in resources)
        {
            // Используем VersionHash для инвалидации кэша браузера (?v=...)
            if (res.Type == WebResourceType.Css) {
                headBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"/{res.RelativePath}?v={res.VersionHash}\" />");
            } 
            else if (res.Type == WebResourceType.Js) {
                bodyBuilder.AppendLine($"<script src=\"/{res.RelativePath}?v={res.VersionHash}\"></script>");
            }
        }

        // 2. Инлайн-скрипты
        foreach (var script in scripts)
        {
            if (script.Position == ScriptPosition.Head) {
                headBuilder.AppendLine($"<script id=\"{script.Id}\">{script.Content}</script>");
            } 
            else if (script.Position == ScriptPosition.BodyEnd) {
                bodyBuilder.AppendLine($"<script id=\"{script.Id}\">{script.Content}</script>");
            }
            else if (script.Position == ScriptPosition.DOMContentLoaded) {
                domReadyScripts.AppendLine(script.Content);
            }
        }

        // 3. Обертка для DOMContentLoaded
        if (domReadyScripts.Length > 0)
        {
            bodyBuilder.AppendLine("<script>");
            bodyBuilder.AppendLine("document.addEventListener('DOMContentLoaded', function() {");
            bodyBuilder.AppendLine(domReadyScripts.ToString());
            bodyBuilder.AppendLine("});");
            bodyBuilder.AppendLine("</script>");
        }

        _headResourcesHtml = headBuilder.ToString();
        _bodyResourcesHtml = bodyBuilder.ToString();
    }

    public override async ValueTask RenderAsync(TextWriter writer, IHttpContext context)
    {
        var model = new
        {
            Title = this.Title,
            Language = this.Language,
            HeadResourcesHtml = _headResourcesHtml,     // Пробрасываем ресурсы в Fluid
            BodyResourcesHtml = _bodyResourcesHtml,     // Пробрасываем ресурсы в Fluid
            HeaderHtml = Header != null ? await RenderChildToStringAsync(Header, context) : "",
            SidebarHtml = Sidebar != null ? await RenderChildToStringAsync(Sidebar, context) : "",
            BreadcrumbsHtml = Breadcrumbs != null ? await RenderChildToStringAsync(Breadcrumbs, context) : "",
            FooterHtml = Footer != null ? await RenderChildToStringAsync(Footer, context) : "",
            MainContentHtml = MainContent != null ? await RenderChildToStringAsync(MainContent, context) : ""
        };

        await RenderCachedTemplateAsync(writer, model);
    }

    private async Task<string> RenderChildToStringAsync(IComponent component, IHttpContext context)
    {
        await using var sw = new StringWriter();
        await component.RenderAsync(sw, context);
        return sw.ToString();
    }

    protected override string GetTemplate() => """
        <!DOCTYPE html>
        <html lang="{{ Language }}">
        <head>
            <meta charset="utf-8" />
            <title>{{ Title }}</title>
            <style>
                body { margin: 0; font-family: system-ui, sans-serif; display: flex; height: 100vh; background: #f8fafc; }
                .main-content { flex: 1; display: flex; flex-direction: column; overflow: hidden; }
                main { padding: 20px; flex: 1; overflow-y: auto; }
            </style>
            
            {{ HeadResourcesHtml | raw }}
        </head>
        <body>
            {{ SidebarHtml | raw }}
            <div class="main-content">
                {{ HeaderHtml | raw }}
                {{ BreadcrumbsHtml | raw }}
                <main>
                    {{ MainContentHtml | raw }}
                </main>
                {{ FooterHtml | raw }}
            </div>
            
            {{ BodyResourcesHtml | raw }}
        </body>
        </html>
        """;
}