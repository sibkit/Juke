using Juke.Web.Core;
using Juke.Web.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Juke.Web.AspNetCore.Http;

public class AspResponseAdapter : IHttpResponse {
    private readonly HttpResponse _response;

    public AspResponseAdapter(HttpResponse response) => _response = response;

    public int StatusCode {
        get => _response.StatusCode;
        set => _response.StatusCode = value;
    }

    public Stream Body => _response.Body;

    public void AddHeader(string key, string value) {
        _response.Headers[key] = value;
    }

    public void SetHeader(string key, string value) {
        _response.Headers[key] = value;
    }

    // <-- Реализация метода чтения
    public string? GetHeader(string key) {
        return _response.Headers.TryGetValue(key, out var values) ? values.ToString() : null;
    }
}