using Fluid;
using Juke.Web.Fluid;

namespace Juke.Web.Components.Fluid;

public class SidebarComponent : FluidComponent {
    public string ActiveMenu { get; init; } = "dashboard";

    // Явно передаем свойство в шаблон
    protected override void ConfigureTemplateContext(TemplateContext context) => context.SetValue("ActiveMenu", ActiveMenu);

    protected override string GetTemplate() =>
        """
        <aside style="width: 250px; background: #1e293b; color: #fff; padding: 20px 0; display: flex; flex-direction: column;">
            <div style="padding: 0 20px 20px; font-size: 1.5rem; font-weight: bold; border-bottom: 1px solid #334155; margin-bottom: 10px;">
                SIBTRONIC
            </div>
            <nav style="display: flex; flex-direction: column;">
                {% assign activeStyle = "padding: 10px 20px; color: #fff; text-decoration: none; background: #3b82f6; border-left: 4px solid #60a5fa;" %}
                {% assign inactiveStyle = "padding: 10px 20px; color: #cbd5e1; text-decoration: none; border-left: 4px solid transparent;" %}

                <a href="/" style="{% if ActiveMenu == 'dashboard' %}{{ activeStyle }}{% else %}{{ inactiveStyle }}{% endif %}">🏠 Dashboard</a>
                <a href="/pcb/new" style="{% if ActiveMenu == 'pcb' %}{{ activeStyle }}{% else %}{{ inactiveStyle }}{% endif %}">➕ Заказы PCB</a>
                <a href="/sales" style="{% if ActiveMenu == 'sales' %}{{ activeStyle }}{% else %}{{ inactiveStyle }}{% endif %}">🛒 Sales</a>
                <a href="/finance" style="{% if ActiveMenu == 'finance' %}{{ activeStyle }}{% else %}{{ inactiveStyle }}{% endif %}">💰 Finance</a>
            </nav>
        </aside>
        """;
}