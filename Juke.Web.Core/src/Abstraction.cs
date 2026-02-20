using System.Security.Claims;

namespace Juke.Web.Core;

//
//
// // --- Resource Abstraction ---
// public enum WebResourceType { Css, Js, Html, Image, Font, Other }
//
// public interface IWebResource
// {
//     string RelativePath { get; } // "css/style.css"
//     WebResourceType Type { get; }
//     Task<Stream> OpenStreamAsync(); // Stream for Zero-IO or Disk reading
//     string VersionHash { get; }
// }
//
// // --- Component Interface ---
// public interface IWebComponent
// {
//     string Id { get; }
//     // Render directly to the stream (Memory efficient)
//     Task RenderAsync(TextWriter writer, IHttpContext context);
//     IEnumerable<IWebResource> GetResources();
// }
//
// public interface IWebPage
// {
//     string RelativePath { get; }
//     WebResourceType Type { get; }
// }