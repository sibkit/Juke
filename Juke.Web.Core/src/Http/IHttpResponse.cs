using System.Text;

namespace Juke.Web.Core.Http;

public interface IHttpResponse {
    int StatusCode { get; set; }
    Stream Body { get; }
    
    void AddHeader(string key, string value); 
    void SetHeader(string key, string value);
    string? GetHeader(string key); // <-- Добавили метод для чтения заголовков
}

public static class HttpResponseExtensions
{
    extension(IHttpResponse response) {
        public void SetContentType(string contentType) {
            response.AddHeader("Content-Type", contentType);
        }

        public string? GetContentType() {
            return response.GetHeader("Content-Type");
        }

        public async Task WriteAsync(string content) {
            var bytes = Encoding.UTF8.GetBytes(content);
            await response.Body.WriteAsync(bytes.AsMemory()); 
        }
    }
}