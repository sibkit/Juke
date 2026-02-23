using System.Reflection;
using Juke.Web.Core.Render;

namespace Juke.Web.Core.Resources;

public class EmbeddedWebResource : IWebResource
{
    private readonly Assembly _assembly;
    private readonly string _manifestResourceName;

    public string RelativePath { get; }
    public WebResourceType Type { get; }
    public string VersionHash { get; }

    public EmbeddedWebResource(
        string relativePath, 
        WebResourceType type, 
        Assembly assembly, 
        string manifestResourceName, 
        string versionHash = "1.0")
    {
        RelativePath = relativePath;
        Type = type;
        _assembly = assembly;
        _manifestResourceName = manifestResourceName;
        VersionHash = versionHash;
    }

    public Task<Stream> OpenStreamAsync()
    {
        var stream = _assembly.GetManifestResourceStream(_manifestResourceName);
        if (stream == null) {
            throw new FileNotFoundException($"Embedded resource '{_manifestResourceName}' not found in assembly '{_assembly.FullName}'.");
        }
        return Task.FromResult(stream);
    }
}