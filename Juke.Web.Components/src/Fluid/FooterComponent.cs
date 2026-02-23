using Juke.Web.Fluid;

namespace Juke.Web.Components.Fluid;

public class FooterComponent : FluidComponent 
{
    protected override string GetTemplate() => """
                                               <footer style="padding: 10px 20px; text-align: center; color: #64748b; border-top: 1px solid #e2e8f0; margin-top: auto;">
                                                   &copy; 2026 Sibtronic ERP. Powered by Juke Web Core.
                                               </footer>
                                               """;
}