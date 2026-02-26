/* Juke.Web.Core/Handlers/PageHandler.cs */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Web.Core.Assets;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;

namespace Juke.Web.Core.Handlers;

public abstract class PageHandler : IRequestHandler 
{
    // Асинхронное создание страницы (отличное место для походов в БД в наследниках)
    protected abstract ValueTask<IPage> CreatePageAsync(IHttpContext context);

    public async Task HandleAsync(IHttpContext context) 
    {
        var page = await CreatePageAsync(context);
        
        // Используем конкретные классы ExternalAsset и InlineAsset
        var externalMap = new Dictionary<string, ExternalAsset>(StringComparer.OrdinalIgnoreCase);
        var inlineMap = new Dictionary<string, InlineAsset>(StringComparer.OrdinalIgnoreCase);
            
        var registry = context.RequestServices.Get<AssetRegistry>();
        CollectDependencies(page, externalMap, inlineMap, registry);

        page.InjectAssets(externalMap.Values.ToList(), inlineMap.Values.ToList());

        await using var writer = new StreamWriter(context.Response.Body, new UTF8Encoding(false), 4096, leaveOpen: true);
        await page.RenderAsync(writer, context);
        await writer.FlushAsync();
    }

    private static void CollectDependencies(
        IComponent node, 
        Dictionary<string, ExternalAsset> externalAssets, 
        Dictionary<string, InlineAsset> inlineAssets,
        AssetRegistry registry) 
    {
        foreach (var asset in node.GetAssets()) 
        {
            switch (asset) 
            {
                case ExternalAsset external:
                    externalAssets.TryAdd(external.RelativePath, external);
                    registry.Register(external); 
                    break;
                case InlineAsset inline:
                    inlineAssets.TryAdd(inline.Id, inline);
                    break;
            }
        }
        if (node is Component baseComponent) {
            foreach (var child in baseComponent.Children) {
                CollectDependencies(child, externalAssets, inlineAssets, registry);
            }
        }
    }
}