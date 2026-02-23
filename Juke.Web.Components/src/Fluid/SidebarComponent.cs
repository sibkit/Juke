using Juke.Web.Fluid;

namespace Juke.Web.Components.Fluid;

public class SidebarComponent : FluidComponent 
{
    protected override string GetTemplate() => """
                                               <aside style="width: 250px; background: #1e293b; color: #fff; padding: 20px 0; display: flex; flex-direction: column;">
                                                   <div style="padding: 0 20px 20px; font-size: 1.5rem; font-weight: bold; border-bottom: 1px solid #334155; margin-bottom: 10px;">
                                                       SIBTRONIC
                                                   </div>
                                                   <nav style="display: flex; flex-direction: column;">
                                                       <a href="/" style="padding: 10px 20px; color: #fff; text-decoration: none; background: #3b82f6;">🏠 Dashboard</a>
                                                       <a href="/sales" style="padding: 10px 20px; color: #cbd5e1; text-decoration: none;">🛒 Sales</a>
                                                       <a href="/finance" style="padding: 10px 20px; color: #cbd5e1; text-decoration: none;">💰 Finance</a>
                                                   </nav>
                                               </aside>
                                               """;
}