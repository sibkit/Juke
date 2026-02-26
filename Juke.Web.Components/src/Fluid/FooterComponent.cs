/* Juke.Web.Components/Fluid/FooterComponent.cs */
using System.Collections.Generic;
using Juke.Web.Core.Assets;
using Juke.Web.Core.Render;
using Juke.Web.Fluid;

namespace Juke.Web.Components.Fluid;

public class FooterComponent : FluidComponent {
    // Заказываем глобальный скрипт сокетов
    public override IReadOnlyList<IAsset> GetAssets() => [
        Assets.WebSockets
    ];

    protected override string GetTemplate() =>
        """
        <footer style="padding: 10px 20px; text-align: center; color: #64748b; border-top: 1px solid #e2e8f0; margin-top: auto; display: flex; justify-content: space-between;">
            <span>&copy; 2026 Sibtronic ERP. Powered by Juke Web Core.</span>
            
            <span style="font-family: monospace; background: #e2e8f0; padding: 2px 8px; border-radius: 4px;">
                Server Time: <strong data-ws-channel="server-time" data-ws-prop="time">--:--:--</strong>
            </span>
        </footer>
        """;
}