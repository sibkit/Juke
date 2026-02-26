namespace Juke.Web.Core.Assets;


public interface IAsset{}

public enum InlinePosition { Head, BodyEnd, DOMContentLoaded }

public class ExternalAsset : IAsset
{
    public string RelativePath { get; }
    public IContent Content { get; }
    public string VersionHash { get; set; } = string.Empty;
    public ExternalAsset(string relativePath, IContent content)
    {
        RelativePath = relativePath;
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}

public class InlineAsset : IAsset
{
    public string Id { get; }
    public InlinePosition Position { get; }
    public StringContent Content { get; }

    public InlineAsset(string id, InlinePosition position, StringContent content)
    {
        Id = id;
        Position = position;
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }
}

public static class Assets {
    public static readonly InlineAsset WebSockets = AssemblyResources.Inline(
        id: "juke-core-ws-client",
        position: InlinePosition.BodyEnd,
        resourcePath: "js/juke-websockets.js",
        assembly: typeof(Assets).Assembly // Берем сборку самого ядра
    );
}