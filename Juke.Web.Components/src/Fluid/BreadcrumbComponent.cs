using Juke.Web.Fluid;

namespace Juke.Web.Components.Fluid;

public class BreadcrumbComponent : FluidComponent 
{
    protected override string GetTemplate() => """
                                               <div style="padding: 10px 20px; background: #f8fafc; border-bottom: 1px solid #e2e8f0; font-size: 0.9rem; color: #64748b;">
                                                   Home / <b>Dashboard</b>
                                               </div>
                                               """;
}