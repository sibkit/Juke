/* Juke.Erp.WebHost/Components/Fluid/MasterPage.cs */
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Juke.Web.Core;
using Juke.Web.Core.Render;
using Juke.Web.Fluid;

namespace Juke.Erp.WebHost.Components;

public class MasterPage : FluidComponent, IPage {
    public string Title { get; set; } = "Sibtronic ERP";
    public string Language { get; set; } = "ru";

    private string _headResourcesHtml = string.Empty;
    private string _bodyResourcesHtml = string.Empty;

    private IComponent? _header;

    public IComponent? Header {
        get => _header;
        set {
            _header = value;
            if (value != null) AddChild(value);
        }
    }

    private IComponent? _sidebar;

    public IComponent? Sidebar {
        get => _sidebar;
        set {
            _sidebar = value;
            if (value != null) AddChild(value);
        }
    }

    private IComponent? _breadcrumbs;

    public IComponent? Breadcrumbs {
        get => _breadcrumbs;
        set {
            _breadcrumbs = value;
            if (value != null) AddChild(value);
        }
    }

    private IComponent? _footer;

    public IComponent? Footer {
        get => _footer;
        set {
            _footer = value;
            if (value != null) AddChild(value);
        }
    }

    private IComponent? _mainContent;

    public IComponent? MainContent {
        get => _mainContent;
        set {
            _mainContent = value;
            if (value != null) AddChild(value);
        }
    }

    public void InjectResources(IReadOnlyList<IWebResource> resources, IReadOnlyList<InlineScript> scripts) {
        // Код инъекции CSS и JS (StringBuilder) остается абсолютно таким же!
        // ... (оставил для краткости, не меняем этот метод)
    }

    public override async ValueTask RenderAsync(TextWriter writer, IHttpContext context) {
        // НИКАКИХ СТРОК И RenderChildToStringAsync!
        // Передаем прямые ссылки на объекты компонентов.
        var model = new {
            Title = this.Title,
            Language = this.Language,
            HeadResourcesHtml = _headResourcesHtml,
            BodyResourcesHtml = _bodyResourcesHtml,

            HeaderComponent = this.Header,
            SidebarComponent = this.Sidebar,
            BreadcrumbsComponent = this.Breadcrumbs,
            FooterComponent = this.Footer,
            MainContentComponent = this.MainContent
        };

        // Обрати внимание: передаем context
        await RenderCachedTemplateAsync(writer, model, context);
    }

    // Обрати внимание на новый синтаксис: {% render PropertyName %}
    protected override string GetTemplate() =>
        """
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
            {% render SidebarComponent %}
            <div class="main-content">
                {% render HeaderComponent %}
                {% render BreadcrumbsComponent %}
                <main>
                    {% render MainContentComponent %}
                </main>
                {% render FooterComponent %}
            </div>
            
            {{ BodyResourcesHtml | raw }}
        </body>
        </html>
        """;
}