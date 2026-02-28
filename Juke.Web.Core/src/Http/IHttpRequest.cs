namespace Juke.Web.Core.Http;

public interface IHttpRequest {
    Method Method { get; }
    string Path { get; } // Removed init, interfaces should define getters for adapters
    string QueryString { get; } 
    
    Dictionary<string, object> RouteValues { get; }
    Stream Body { get; }

    // Direct method is faster and safer than allocating dictionaries
    string? GetHeader(string key); 
    
    bool HasFormContentType { get; }
    IEnumerable<IUploadedFile> Files { get; }
}