/* Juke.Web.Core/Handlers/PageHandler.cs */
using System.Text;
using System.Threading.Tasks;
using Juke.Web.Core.Render;

namespace Juke.Web.Core.Handlers;

public abstract class PageHandler : IRequestHandler 
{
    // Теперь метод асинхронный, чтобы можно было сходить в БД!
    protected abstract ValueTask<IPage> CreatePageAsync(IHttpContext context);

    public async Task HandleAsync(IHttpContext context) 
    {
        // 1. Сбор данных и создание дерева страницы происходит здесь
        var page = await CreatePageAsync(context);

        // 2. Сбор ресурсов со всего готового дерева (Дедупликация)
        var resourcesMap = new Dictionary<string, IWebResource>(StringComparer.OrdinalIgnoreCase);
        var scriptsMap = new Dictionary<string, InlineScript>(StringComparer.OrdinalIgnoreCase);
            
        CollectDependencies(page, resourcesMap, scriptsMap);

        // 3. Инъекция собранных данных обратно в страницу
        page.InjectResources(resourcesMap.Values.ToList(), scriptsMap.Values.ToList());

        // 4. Потоковый рендер
        await using var writer = new StreamWriter(context.Response.Body, new UTF8Encoding(false), 4096, leaveOpen: true);
        await page.RenderAsync(writer, context);
        await writer.FlushAsync();
    }

    private void CollectDependencies(
        IComponent node, 
        Dictionary<string, IWebResource> resources, 
        Dictionary<string, InlineScript> scripts) 
    {
        var nodeResources = node.GetResources();
        foreach (var res in nodeResources) {
            resources.TryAdd(res.RelativePath, res);
        }

        var nodeScripts = node.GetInlineScripts();
        foreach (var script in nodeScripts) {
            scripts.TryAdd(script.Id, script);
        }

        if (node is Component baseComponent) {
            var children = baseComponent.Children;
            foreach (var t in children) {
                CollectDependencies(t, resources, scripts);
            }
        }
    }
}