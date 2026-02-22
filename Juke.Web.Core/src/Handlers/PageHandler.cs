using System.Text;
using Juke.Web.Core.Render;

namespace Juke.Web.Core.Handlers;

public abstract class PageHandler : IRequestHandler {
        
        protected abstract IPage CreatePage(IHttpContext context);

        public async Task HandleAsync(IHttpContext context) {
            // 1. Создаем страницу
            var page = CreatePage(context);

            // 2. Инициализация (дерево строится, данные грузятся)
            await page.InitAsync(context);

            // 3. Сбор ресурсов со всего готового дерева (Дедупликация)
            var resourcesMap = new Dictionary<string, IWebResource>(StringComparer.OrdinalIgnoreCase);
            var scriptsMap = new Dictionary<string, InlineScript>(StringComparer.OrdinalIgnoreCase);
            
            CollectDependencies(page, resourcesMap, scriptsMap);

            // 4. Инъекция собранных данных обратно в страницу
            page.InjectDependencies(resourcesMap.Values.ToList(), scriptsMap.Values.ToList());

            // 5. Потоковый рендер
            await using var writer = new StreamWriter(context.Response.Body, new UTF8Encoding(false), 4096, leaveOpen: true);
            // Теперь страница сама рендерит весь документ, включая ресурсы!
            await page.RenderAsync(writer, context);
            await writer.FlushAsync();
        }

        private void CollectDependencies(
            IComponent node, 
            Dictionary<string, IWebResource> resources, 
            Dictionary<string, InlineScript> scripts) {
            
            var nodeResources = node.GetResources();
            for (int i = 0; i < nodeResources.Count; i++) {
                var res = nodeResources[i];
                resources.TryAdd(res.RelativePath, res);
            }

            var nodeScripts = node.GetInlineScripts();
            for (int i = 0; i < nodeScripts.Count; i++) {
                var script = nodeScripts[i];
                scripts.TryAdd(script.Id, script);
            }

            // Обходим детей
            if (node is Component baseComponent) {
                var children = baseComponent.Children;
                for (int i = 0; i < children.Count; i++) {
                    CollectDependencies(children[i], resources, scripts);
                }
            }
        }


}