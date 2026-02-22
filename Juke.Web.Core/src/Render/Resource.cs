namespace Juke.Web.Core.Render;



// Root component marker.


public enum WebResourceType { Css, Js, Html, Image, Font, Other }

public interface IWebResource
{
    string RelativePath { get; } // "css/style.css"
    WebResourceType Type { get; }
    Task<Stream> OpenStreamAsync(); // Stream for Zero-IO or Disk reading
    string VersionHash { get; }
}