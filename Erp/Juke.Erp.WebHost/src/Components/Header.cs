using Juke.Web.Fluid;

namespace Juke.Erp.WebHost.Components;

public class Header: FluidComponent 
{
    // Data injected from the outside
    public required string CurrentUser { get; init; }
    public string CurrentTheme { get; init; } = "Light";

    // Pass the current state to Fluid
    protected override object? GetModel() => new { CurrentUser, CurrentTheme };

    protected override string GetTemplate() => """
                                               <header style="background: #fff; padding: 15px 20px; display: flex; justify-content: space-between; border-bottom: 1px solid #e2e8f0;">
                                                   <div style="font-weight: bold; font-size: 1.2rem;">Juke ERP</div>
                                                   <div>
                                                       <span style="margin-right: 15px;">👤 {{ CurrentUser }}</span>
                                                       <button style="padding: 5px 10px; cursor: pointer;">Logout</button>
                                                   </div>
                                               </header>
                                               """;
}