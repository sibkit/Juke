/* Juke.Erp.WebHost/Components/DashboardContent.cs */
using System.Collections.Generic;
using Juke.Web.Core.Assets;
using Juke.Web.Core.Render;
using Juke.Web.Fluid;

namespace Juke.Erp.WebHost.Components;

public class DashboardContent : FluidComponent {
    // Данные, которые передаются в компонент при его создании
    public required string StoreId { get; init; }
    public required string StoreName { get; init; }
    public required decimal SalesToday { get; init; }
    
    // Запрашиваем внедрение системного скрипта для авто-биндинга
    public override IReadOnlyList<IAsset> GetAssets() => [
        Assets.WebSockets
    ];

    protected override string GetTemplate() =>
        """
        <style>
            /* Специфичная для компонента анимация подсветки при получении данных по сокету */
            .ws-flash-update { animation: textFlash 1s ease-out; }
            @keyframes textFlash { 
                0% { color: #22c55e; transform: scale(1.05); } 
                100% { color: inherit; transform: scale(1); } 
            }
        </style>

        <div style="padding: 20px; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1);">
            <h3 style="margin-top: 0; color: #475569; font-size: 1rem;">Sales Today ({{ StoreName }})</h3>
            
            <p style="font-size: 28px; font-weight: bold; margin: 10px 0 0 0; color: #0f172a; display: inline-block;"
               data-ws-channel="sales-{{ StoreId }}" 
               data-ws-prop="newTotal"
               data-ws-flash="ws-flash-update">
                ${{ SalesToday }}
            </p>
        </div>
        """;
}