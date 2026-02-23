using Juke.Web.Fluid;

namespace Juke.Erp.WebHost.Components;

public class DashboardContent : FluidComponent 
{
    // External data requirements
    public required decimal SalesToday { get; init; }
    public required int ActiveUsers { get; init; }
    public required int SystemAlerts { get; init; }

    // Since the properties match the template variables, we can just pass 'this'
    protected override object? GetModel() => this;

    protected override string GetTemplate() => """
                                               <h1 style="margin-top: 0; color: #0f172a;">Welcome to Sibtronic</h1>
                                               <p style="color: #475569;">System is fully operational. Modular architecture active.</p>

                                               <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px; margin-top: 20px;">
                                                   <div style="padding: 20px; background: #eff6ff; border: 1px solid #bfdbfe; border-radius: 6px;">
                                                       <h3>Sales Today</h3>
                                                       <p style="font-size: 24px; font-weight: bold; color: #1d4ed8;">${{ SalesToday }}</p>
                                                   </div>
                                                   <div style="padding: 20px; background: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 6px;">
                                                       <h3>Active Users</h3>
                                                       <p style="font-size: 24px; font-weight: bold; color: #15803d;">{{ ActiveUsers }}</p>
                                                   </div>
                                                   <div style="padding: 20px; background: #fef2f2; border: 1px solid #fecaca; border-radius: 6px;">
                                                       <h3>System Alerts</h3>
                                                       <p style="font-size: 24px; font-weight: bold; color: #b91c1c;">{{ SystemAlerts }}</p>
                                                   </div>
                                               </div>
                                               """;
}