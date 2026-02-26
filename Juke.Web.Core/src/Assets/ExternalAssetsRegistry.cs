using System.Collections.Concurrent;

namespace Juke.Web.Core.Assets;

public class AssetRegistry {
    // Используем класс ExternalAsset вместо IExternalAsset
    private readonly ConcurrentDictionary<string, ExternalAsset> _assets = new(StringComparer.OrdinalIgnoreCase);

    public void Register(ExternalAsset asset) {
        _assets.TryAdd(asset.RelativePath, asset);
    }

    public bool TryGet(string relativePath, out ExternalAsset? asset) {
        return _assets.TryGetValue(relativePath, out asset);
    }
}