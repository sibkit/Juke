using Juke.Web.Fluid;

namespace Flux.Pcb.Web.Components;

public class PcbUploadForm : FluidComponent {
    protected override string GetTemplate() =>
        """
        <div style="background: #fff; padding: 20px; border-radius: 8px; border: 1px solid #e2e8f0; max-width: 500px;">
            <h2 style="margin-top: 0; color: #0f172a;">Анализ Gerber-архива</h2>
            <p style="color: #64748b; margin-bottom: 20px;">Загрузите ZIP-архив с герберами, чтобы система извлекла слои и рассчитала габариты.</p>
            
            <form action="/pcb/upload" method="post" enctype="multipart/form-data">
                <div style="margin-bottom: 15px;">
                    <label style="display: block; margin-bottom: 5px; font-weight: bold; color: #475569;">Архив проекта (.zip):</label>
                    <input type="file" name="archive" accept=".zip" required 
                           style="width: 100%; padding: 8px; border: 1px solid #cbd5e1; border-radius: 4px; box-sizing: border-box;" />
                </div>
                <button type="submit" style="background: #3b82f6; color: white; border: none; padding: 10px 20px; border-radius: 4px; cursor: pointer; font-size: 1rem;">
                    Загрузить и проанализировать
                </button>
            </form>
        </div>
        """;
}