/* PcbViewerSideBar.cs */
using System;
using System.Collections.Generic;
using Fluid;
using Juke.Web.Fluid;

namespace Flux.Pcb.Web.Components;

public class PcbViewerSidebar : FluidComponent
{
    public required Guid OrderId { get; init; }
    public required List<LayerViewModel> LayerModels { get; init; }
    public required List<string> FailedLayers { get; init; }
    public required List<string> IgnoredFiles { get; init; } // Добавлено

    protected override void ConfigureTemplateContext(TemplateContext context)
    {
        context.SetValue("OrderId", OrderId.ToString());
        context.SetValue("LayerModels", LayerModels);
        context.SetValue("FailedLayers", FailedLayers);
        context.SetValue("IgnoredFiles", IgnoredFiles);
    }

    protected override string GetTemplate() => """
        <div class="layers-panel" style="width: 300px; background: #1e293b; border: 1px solid #334155; border-radius: 8px; padding: 15px; overflow-y: auto; user-select: none;">
            <h3 style="margin-top: 0; color: #cbd5e1; font-size: 1rem; border-bottom: 1px solid #334155; padding-bottom: 10px;">Слои проекта</h3>
            
            {% if LayerModels.size == 0 %}
                <p style="color: #94a3b8; font-size: 0.9rem;">Нет успешно загруженных слоев.</p>
            {% else %}
                {% for lm in LayerModels %}
                    <div class="layer-item" style="display: flex; align-items: center; gap: 10px; margin-bottom: 10px; padding: 8px; background: #0f172a; border-radius: 6px; border-left: 4px solid {{ lm.ColorHex }};">
                        <input type="checkbox" id="chk-{{ lm.SafeId }}" checked onchange="toggleLayer('{{ lm.SafeId }}', this.checked)">
                        <label for="chk-{{ lm.SafeId }}" title="{{ lm.FileName }}" style="cursor: pointer; font-size: 0.85rem; font-family: monospace; word-break: break-all; flex: 1; color: #e2e8f0;">{{ lm.FileName }}</label>
                        <a href="{{ lm.Url }}" target="_blank" title="Открыть SVG" style="text-decoration: none; font-size: 1.1rem; filter: grayscale(1); transition: 0.2s;">📄</a>
                    </div>
                {% endfor %}
            {% endif %}

            {% if IgnoredFiles.size > 0 %}
                <h3 style="margin-top: 20px; color: #93c5fd; font-size: 1rem; border-bottom: 1px solid #1e3a8a; padding-bottom: 10px;">Пропущенные файлы</h3>
                {% for ignored in IgnoredFiles %}
                    <div style="display: flex; align-items: center; justify-content: space-between; font-size: 0.8rem; font-family: monospace; color: #bfdbfe; background: #172554; padding: 5px 8px; border-radius: 4px; margin-bottom: 5px;">
                        <span style="word-break: break-all; margin-right: 10px;">ℹ️ {{ ignored }}</span>
                        <a href="/pcb/api/order/{{ OrderId }}/raw/{{ ignored }}" target="_blank" style="text-decoration: none; font-size: 1.1rem; flex-shrink: 0;" title="Скачать/Посмотреть файл">📥</a>
                    </div>
                {% endfor %}
            {% endif %}

            {% if FailedLayers.size > 0 %}
                <h3 style="margin-top: 20px; color: #ef4444; font-size: 1rem; border-bottom: 1px solid #7f1d1d; padding-bottom: 10px;">Ошибки парсинга</h3>
                {% for failed in FailedLayers %}
                    <div style="font-size: 0.8rem; font-family: monospace; color: #fca5a5; background: #450a0a; padding: 5px 8px; border-radius: 4px; margin-bottom: 5px; word-break: break-all;">
                        ❌ {{ failed }}
                    </div>
                {% endfor %}
            {% endif %}
        </div>
        """;
}