/* PcbViewerCanvas.cs */
using System.Collections.Generic;
using Fluid;
using Juke.Web.Fluid;

namespace Flux.Pcb.Web.Components;

public class PcbViewerCanvas : FluidComponent
{
    public required List<LayerViewModel> LayerModels { get; init; }

    protected override void ConfigureTemplateContext(TemplateContext context) => context.SetValue("LayerModels", LayerModels);

    protected override string GetTemplate() => """
        <div class="pcb-canvas" id="canvas-container" style="flex: 1; position: relative; background-color: #020617; border-radius: 8px; border: 1px solid #334155; overflow: hidden; cursor: grab;">
            <div class="zoom-controls" style="position: absolute; top: 15px; left: 15px; z-index: 10; display: flex; flex-direction: column; gap: 8px;">
                <button class="zoom-btn" onclick="zoomByButton(1.3)" title="Увеличить" style="width: 36px; height: 36px; background: #1e293b; color: white; border: 1px solid #475569; border-radius: 6px; cursor: pointer;">+</button>
                <button class="zoom-btn" onclick="zoomByButton(0.76)" title="Уменьшить" style="width: 36px; height: 36px; background: #1e293b; color: white; border: 1px solid #475569; border-radius: 6px; cursor: pointer;">−</button>
            </div>
            
            <div class="grid-indicator" id="grid-indicator-text" style="position: absolute; bottom: 15px; left: 15px; z-index: 10; background: rgba(30, 41, 59, 0.9); color: #cbd5e1; padding: 6px 12px; border-radius: 6px; border: 1px solid #475569; font-family: monospace; font-size: 0.85rem;">Загрузка...</div>

            <canvas id="grid-canvas" style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; pointer-events: none; z-index: 1;"></canvas>
            <div id="board-workspace" style="position: absolute; top: 0; left: 0; transform-origin: 0 0; pointer-events: none; z-index: 2;"></div>
        </div>
        
        <style>
            .pcb-canvas:active { cursor: grabbing !important; }
            .zoom-btn:hover { background: #334155 !important; }
            /* Удален хак .fast-pan, оставляем только постоянный screen режим наложения */
            .pcb-layer-img { position: absolute; top: 0; left: 0; width: 100%; height: 100%; object-fit: contain; mix-blend-mode: screen; opacity: 0.9; }
        </style>

        <script>
            function toggleLayer(safeId, isVisible) {
                var el = document.getElementById('render-' + safeId);
                if (el) el.style.display = isVisible ? 'block' : 'none';
            }

            let scale = 1, panX = 0, panY = 0;
            let container, workspace, gridCanvas, ctx, indicatorText;
            let isTicking = false, isFirstLayer = true;
            
            let svgScaleFactor = 1; 
            
            // Данные реальных координат CAD-системы
            let cadVx = 0, cadVy = 0, cadVw = 100, cadVh = 100;
            let cadUnitToMm = 1.0; 

            function requestUpdate() { if (!isTicking) { window.requestAnimationFrame(renderFrame); isTicking = true; } }

            function renderFrame() {
                isTicking = false;
                workspace.style.transform = `translate(${panX}px, ${panY}px) scale(${scale / svgScaleFactor})`;
                
                ctx.clearRect(0, 0, gridCanvas.width, gridCanvas.height);

                // Стандартные CAD-шаги (в миллиметрах)
                const steps = [0.001, 0.005, 0.01, 0.05, 0.1, 0.5, 1, 5, 10, 50, 100, 500, 1000];
                
                // Переводим шаг в экранные пиксели (с учетом дюймов/мм оригинального файла)
                const pxPerMm = scale / cadUnitToMm;
                let minorStepMm = steps.find(s => s * pxPerMm >= 20) || 100;
                let minorStepCAD = minorStepMm / cadUnitToMm;

                // Вычисляем видимую область математически (в CAD координатах)
                const cadStartX = cadVx + (0 - panX) / scale;
                const cadEndX   = cadVx + (gridCanvas.width - panX) / scale;
                
                // Ось Y в CAD инвертирована относительно DOM
                const cadMaxY = cadVy + cadVh + panY / scale;
                const cadMinY = cadVy + cadVh - (gridCanvas.height - panY) / scale;

                const startIdxX = Math.floor(cadStartX / minorStepCAD);
                const endIdxX   = Math.ceil(cadEndX / minorStepCAD);
                const startIdxY = Math.floor(cadMinY / minorStepCAD);
                const endIdxY   = Math.ceil(cadMaxY / minorStepCAD);

                function drawGrid(isMajor) {
                    ctx.beginPath();
                    for (let i = startIdxX; i <= endIdxX; i++) {
                        if (isMajor ? (i % 10 !== 0) : (i % 10 === 0)) continue;
                        let cadX = i * minorStepCAD;
                        let screenX = Math.floor(panX + (cadX - cadVx) * scale) + 0.5;
                        ctx.moveTo(screenX, 0); ctx.lineTo(screenX, gridCanvas.height);
                    }
                    for (let i = startIdxY; i <= endIdxY; i++) {
                        if (isMajor ? (i % 10 !== 0) : (i % 10 === 0)) continue;
                        let cadY = i * minorStepCAD;
                        let screenY = Math.floor(panY + (cadVy + cadVh - cadY) * scale) + 0.5;
                        ctx.moveTo(0, screenY); ctx.lineTo(gridCanvas.width, screenY);
                    }
                    ctx.stroke();
                }

                // Минорная сетка
                ctx.strokeStyle = 'rgba(148, 163, 184, 0.12)';
                ctx.lineWidth = 1;
                drawGrid(false);

                // Мажорная сетка
                ctx.strokeStyle = 'rgba(148, 163, 184, 0.35)';
                ctx.lineWidth = 1;
                drawGrid(true);

                // Отрисовка осей координат (Истинный 0,0 инженера)
                ctx.beginPath();
                let screenOriginX = Math.floor(panX + (0 - cadVx) * scale) + 0.5;
                let screenOriginY = Math.floor(panY + (cadVy + cadVh - 0) * scale) + 0.5;

                if (screenOriginX >= 0 && screenOriginX <= gridCanvas.width) {
                    ctx.moveTo(screenOriginX, 0); ctx.lineTo(screenOriginX, gridCanvas.height);
                }
                if (screenOriginY >= 0 && screenOriginY <= gridCanvas.height) {
                    ctx.moveTo(0, screenOriginY); ctx.lineTo(gridCanvas.width, screenOriginY);
                }
                ctx.strokeStyle = 'rgba(239, 68, 68, 0.5)';
                ctx.lineWidth = 1;
                ctx.stroke();

                if (indicatorText) {
                    indicatorText.innerText = `Сетка: ${minorStepMm} мм`;
                }
            }

            window.zoomByButton = function(f) {
                const rect = container.getBoundingClientRect();
                const cx = rect.width / 2; const cy = rect.height / 2;
                const tx = (cx - panX) / scale; const ty = (cy - panY) / scale;
                scale *= f; scale = Math.max(0.01, Math.min(scale, 10000));
                panX = cx - tx * scale; panY = cy - ty * scale; requestUpdate();
            };

            document.addEventListener('DOMContentLoaded', () => {
                container = document.getElementById('canvas-container'); workspace = document.getElementById('board-workspace');
                gridCanvas = document.getElementById('grid-canvas'); indicatorText = document.getElementById('grid-indicator-text');
                if (!container) return; ctx = gridCanvas.getContext('2d');

                const layers = [ {% for lm in LayerModels %} { id: '{{ lm.SafeId }}', url: '{{ lm.Url }}', color: '{{ lm.ColorHex }}' }, {% endfor %} ];

                layers.forEach(lm => {
                    fetch(lm.url).then(r => r.text()).then(svgText => {
                        if (isFirstLayer && svgText.includes('viewBox')) {
                            const match = svgText.match(/viewBox="([^"]+)"/i);
                            if (match) {
                                [cadVx, cadVy, cadVw, cadVh] = match[1].split(/[\s,]+/).map(Number);
                                
                                const uomMatch = svgText.match(/data-uom="([^"]+)"/i);
                                if (uomMatch && uomMatch[1].toLowerCase() === 'inch') {
                                    cadUnitToMm = 25.4;
                                } else {
                                    cadUnitToMm = 1.0;
                                }
                                
                                svgScaleFactor = 5000 / Math.max(cadVw, cadVh);
                                workspace.style.width = (cadVw * svgScaleFactor) + 'px'; 
                                workspace.style.height = (cadVh * svgScaleFactor) + 'px';
                                
                                const rect = container.getBoundingClientRect();
                                scale = Math.min(rect.width / cadVw, rect.height / cadVh) * 0.8;
                                panX = (rect.width - cadVw * scale) / 2; panY = (rect.height - cadVh * scale) / 2;
                                isFirstLayer = false;
                                requestUpdate(); 
                            }
                        }
                        const coloredText = svgText.replace(/(fill|stroke)="([^"]+)"/gi, (m, a, v) => v.toLowerCase() === 'none' ? m : `${a}="${lm.color}"`);
                        const img = document.createElement('img'); img.id = 'render-' + lm.id; img.className = 'pcb-layer-img';
                        img.src = URL.createObjectURL(new Blob([coloredText], { type: 'image/svg+xml' }));
                        workspace.appendChild(img);
                    });
                });

                function resizeCanvas() { gridCanvas.width = container.offsetWidth; gridCanvas.height = container.offsetHeight; requestUpdate(); }
                window.addEventListener('resize', resizeCanvas); resizeCanvas();

                container.addEventListener('wheel', e => {
                    e.preventDefault(); const rect = container.getBoundingClientRect();
                    const tx = (e.clientX - rect.left - panX) / scale; const ty = (e.clientY - rect.top - panY) / scale;
                    scale *= (-Math.sign(e.deltaY) > 0) ? 1.15 : (1/1.15); scale = Math.max(0.01, Math.min(scale, 10000)); 
                    panX = (e.clientX - rect.left) - tx * scale; panY = (e.clientY - rect.top) - ty * scale; requestUpdate();
                }, { passive: false });

                let isDragging = false, startX, startY;
                container.addEventListener('mousedown', e => {
                    if (e.target.closest('.zoom-controls') || e.button !== 0) return;
                    isDragging = true; startX = e.clientX - panX; startY = e.clientY - panY;
                });
                window.addEventListener('mousemove', e => { if (isDragging) { panX = e.clientX - startX; panY = e.clientY - startY; requestUpdate(); }});
                window.addEventListener('mouseup', () => isDragging = false); container.addEventListener('mouseleave', () => isDragging = false);
            });
        </script>
        """;
}