using Fluid;
using Juke.Web.Core.Render;
using Juke.Web.Fluid;

namespace Flux.Host.Pages;

public class MasterPage : FluidPage {
    // Регионы Layout'а
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

    protected override void ConfigureTemplateContext(TemplateContext context) 
    {
        base.ConfigureTemplateContext(context); // Важно, чтобы прокинулись Title и Assets!
        
        // Явно передаем компоненты-регионы, чтобы Fluid их увидел
        context.SetValue("Header", Header);
        context.SetValue("Sidebar", Sidebar);
        context.SetValue("Breadcrumbs", Breadcrumbs); 
        context.SetValue("Footer", Footer);
        context.SetValue("MainContent", MainContent);
    }
    
    // Просто пробрасываем сам класс как модель!
    protected override object? GetModel() => this;

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
            
            {{ HeadAssetsHtml | raw }}
        </head>
        <body>
            {% render Sidebar %}
            <div class="main-content">
                {% render Header %}
                {% render Breadcrumbs %}
                <main>
                    {% render MainContent %}
                </main>
                {% render Footer %}
            </div>
            
            {{ BodyAssetsHtml | raw }}
        </body>
        </html>
        """;
}