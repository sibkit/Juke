using Fluid;
using Juke.Web.Core.Render;
using Juke.Web.Fluid;

namespace Juke.Web.Components.Fluid;

public class BreadcrumbComponent : FluidComponent {
    // 1. РАЗРЕШАЕМ FLUID ЧИТАТЬ СВОЙСТВА КЛАССА
    static BreadcrumbComponent() {
        TemplateOptions.Default.MemberAccessStrategy.Register<BreadcrumbItem>();
    }

    public required List<BreadcrumbItem> Items { get; init; }

    // 2. Явно передаем список
    protected override void ConfigureTemplateContext(TemplateContext context) {
        context.SetValue("Items", Items);
    }

    protected override string GetTemplate() =>
        """
        <div style="padding: 10px 20px; background: #f8fafc; border-bottom: 1px solid #e2e8f0; font-size: 0.9rem; color: #64748b;">
            {% for item in Items %}
                {% if item.Url %}
                    <a href="{{ item.Url }}" style="color: #3b82f6; text-decoration: none;">{{ item.Label }}</a>
                    <span style="margin: 0 5px;">/</span>
                {% else %}
                    <b style="color: #0f172a;">{{ item.Label }}</b>
                {% endif %}
            {% endfor %}
        </div>
        """;
}