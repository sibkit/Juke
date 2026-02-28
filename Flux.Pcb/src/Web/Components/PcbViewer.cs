/* PcbViewer.cs */
using System;
using System.Collections.Generic;
using System.Linq;
using Fluid;
using Juke.Web.Fluid;

namespace Flux.Pcb.Web.Components;

public class PcbViewer : FluidComponent
{
    static PcbViewer() { TemplateOptions.Default.MemberAccessStrategy.Register<LayerViewModel>(); }

    public required Guid OrderId { get; init; }
    public required List<string> Layers { get; init; } = [];
    public required List<string> FailedLayers { get; init; } = [];
    public required List<string> IgnoredFiles { get; init; } = []; // Добавляем новое поле

    private string DetermineColorHex(string fileName)
    {
        var n = fileName.ToLowerInvariant();
        if (n.Contains(".gtl") || n.Contains(".gbl") || n.Contains("copper") || n.Contains("cu")) return "#d97706"; 
        if (n.Contains(".gts") || n.Contains(".gbs") || n.Contains("mask")) return "#15803d"; 
        if (n.Contains(".gto") || n.Contains(".gbo") || n.Contains("silk")) return "#ffffff"; 
        if (n.Contains(".gko") || n.Contains("edge") || n.Contains("outline")) return "#eab308"; 
        if (n.Contains(".drl") || n.Contains(".txt") || n.Contains("drill")) return "#ef4444"; 
        return "#94a3b8";
    }

    protected override void ConfigureTemplateContext(TemplateContext context) 
    {
        var viewModels = Layers.Select((fileName, index) => new LayerViewModel {
            SafeId = $"layer_{index}", FileName = fileName, Url = $"/pcb/api/order/{OrderId}/layer/{fileName}", ColorHex = DetermineColorHex(fileName)
        }).ToList();

        // Прокидываем OrderId и IgnoredFiles в Sidebar
        context.SetValue("Sidebar", new PcbViewerSidebar { 
            OrderId = OrderId,
            LayerModels = viewModels, 
            FailedLayers = FailedLayers,
            IgnoredFiles = IgnoredFiles 
        });
        
        context.SetValue("Canvas", new PcbViewerCanvas { LayerModels = viewModels });
    }

    protected override string GetTemplate() => """
                                               <div class="viewer-layout" style="display: flex; gap: 20px; height: 75vh; min-height: 600px;">
                                                   {% render Sidebar %}
                                                   {% render Canvas %}
                                               </div>
                                               """;
}