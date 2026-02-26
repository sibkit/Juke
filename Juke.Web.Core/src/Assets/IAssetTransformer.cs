namespace Juke.Web.Core.Assets;

public interface IAssetTransformer
{
    string Transform(string content, StringContentType type);
}