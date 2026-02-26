using System.Collections.Concurrent;

namespace Juke.Web.Core.Assets;

public class AssetCache {
    private readonly IEnumerable<IAssetTransformer> _transformers;
    private readonly ConcurrentDictionary<string, string> _cache = new(StringComparer.Ordinal);

    public AssetCache(IEnumerable<IAssetTransformer>? transformers) {
        _transformers = transformers ?? [];
    }

    public string GetProcessedContent(InlineAsset asset) {
        // Берем класс InlineAsset
        if (_cache.TryGetValue(asset.Id, out var processedContent)) return processedContent;

        var content = asset.Content.Text; // Достаем текст из StringContent!

        if (!string.IsNullOrWhiteSpace(content)) {
            foreach (var transformer in _transformers) {
                // Передаем тип строкового контента
                content = transformer.Transform(content, asset.Content.Type);
            }
        }

        content ??= string.Empty;
        _cache.TryAdd(asset.Id, content);

        return content;
    }
}