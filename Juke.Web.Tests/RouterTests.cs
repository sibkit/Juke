/* RouterTests.cs */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Juke.Web.Core;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;
using Juke.Web.Core.Routing;

namespace Juke.Web.Tests;

// --- Заглушки для тестов ---

public class DummyHandler : IRequestHandler 
{ 
    public string Name { get; }
    public DummyHandler(string name = "") { Name = name; }

    public Task HandleAsync(IHttpContext context) {
        return Task.CompletedTask;
    }
}

public class DummyErrorHandler : IErrorHandler 
{
    public Task HandleAsync(IHttpContext context, Exception? exception) {
        return Task.CompletedTask;
    }
}

public class AnyStringMatcher : IPathPartMatcher 
{
    public bool TryMatch(ReadOnlySpan<char> pathPart, out object? parsedValue) {
        if (pathPart.IsEmpty) {
            parsedValue = null;
            return false;
        }
        parsedValue = pathPart.ToString();
        return true;
    }
}

public class MockHttpRequest : IHttpRequest 
{
    public Method Method { get; set; } 
    public string Path { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public Dictionary<string, object> RouteValues { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Stream Body { get; set; } = Stream.Null;

    private readonly Dictionary<string, string> _headers = new(StringComparer.OrdinalIgnoreCase);
    
    public string? GetHeader(string key) => _headers.TryGetValue(key, out var val) ? val : null;
    public void SetHeaderForTest(string key, string value) => _headers[key] = value;
}

public class MockHttpResponse : IHttpResponse 
{
    public int StatusCode { get; set; } = 200;
    public Stream Body { get; } = new MemoryStream();

    public Dictionary<string, string> InternalHeaders { get; } = new(StringComparer.OrdinalIgnoreCase);

    public void AddHeader(string key, string value) {
        InternalHeaders[key] = value;
    }

    public string? GetHeader(string key) {
        return InternalHeaders.TryGetValue(key, out var value) ? value : null;
    }
}

public class DummyServiceProvider : IServiceProvider 
{
    private readonly Dictionary<Type, object> _services = new();
    public void AddService<T>(T service) where T : notnull => _services[typeof(T)] = service;
    public object? GetService(Type serviceType) => _services.GetValueOrDefault(serviceType);
}

public class MockHttpContext : IHttpContext 
{
    public IHttpRequest Request { get; set; } = new MockHttpRequest();
    public IHttpResponse Response { get; } = new MockHttpResponse();
    
    // Теперь мы можем прокидывать сервисы
    public IServiceProvider RequestServices { get; set; } = new DummyServiceProvider(); 
    public IWebSocketManager WebSockets { get; set; } = null!; // Заглушка, если сокеты не тестируем
}

// --- Сами тесты ---

public class RouterTests
{
    private Router SetupRouter()
    {
        var root = new GroupRouteNode();
        var router = new Router(root) {
            ErrorHandlers = {
                [404] = new DummyErrorHandler(),
                [400] = new DummyErrorHandler()
            }
        };

        // 1. Главная страница: /
        root.AddHandler(Method.GET, new DummyHandler("Root"));

        // 2. Статический путь: /api/users
        var apiNode = new StaticRouteNode("api");
        var usersNode = new StaticRouteNode("users");
        usersNode.AddHandler(Method.GET, new DummyHandler("GetUsers"));
        apiNode.AddNode(usersNode);

        // 3. Динамический путь: /api/users/{id}
        var userIdNode = new DynamicRouteNode(new AnyStringMatcher(), "id");
        userIdNode.AddHandler(Method.GET, new DummyHandler("GetUserById"));
        usersNode.AddNode(userIdNode);

        // 4. Группа (монтирование): /api/v1/orders
        var v1Group = new GroupRouteNode();
        var ordersNode = new StaticRouteNode("orders");
        ordersNode.AddHandler(Method.POST, new DummyHandler("CreateOrder"));
        v1Group.AddNode(ordersNode);
        apiNode.Mount("v1", v1Group);

        root.AddNode(apiNode);

        return router;
    }

    [Fact]
    public void Resolve_RootPath_ReturnsRootHandler()
    {
        var router = SetupRouter();
        var context = new MockHttpContext { Request = new MockHttpRequest { Method = Method.GET, Path = "/" } };

        var handler = router.Resolve(context) as DummyHandler;

        Assert.NotNull(handler);
        Assert.Equal("Root", handler.Name);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public void Resolve_StaticPath_ReturnsCorrectHandler()
    {
        var router = SetupRouter();
        var context = new MockHttpContext { Request = new MockHttpRequest { Method = Method.GET, Path = "/api/users" } };

        var handler = router.Resolve(context) as DummyHandler;

        Assert.NotNull(handler);
        Assert.Equal("GetUsers", handler.Name);
    }

    [Fact]
    public void Resolve_DynamicPath_ReturnsHandler_AndSetsRouteValues()
    {
        var router = SetupRouter();
        var context = new MockHttpContext { Request = new MockHttpRequest { Method = Method.GET, Path = "/api/users/42" } };

        var handler = router.Resolve(context) as DummyHandler;

        Assert.NotNull(handler);
        Assert.Equal("GetUserById", handler.Name);
        Assert.True(context.Request.RouteValues.ContainsKey("id"));
        Assert.Equal("42", context.Request.RouteValues["id"]);
    }

    [Fact]
    public void Resolve_MountedGroup_ResolvesCorrectly()
    {
        var router = SetupRouter();
        var context = new MockHttpContext { Request = new MockHttpRequest { Method = Method.POST, Path = "/api/v1/orders" } };

        var handler = router.Resolve(context) as DummyHandler;

        Assert.NotNull(handler);
        Assert.Equal("CreateOrder", handler.Name);
    }

    [Fact]
    public void Resolve_NotFoundPath_Returns404ErrorHandler_AndSetsStatusCode()
    {
        var router = SetupRouter();
        var context = new MockHttpContext { Request = new MockHttpRequest { Method = Method.GET, Path = "/api/unknown" } };

        var handler = router.Resolve(context);

        Assert.IsType<DummyErrorHandler>(handler);
        Assert.Equal(404, context.Response.StatusCode);
    }

    [Fact]
    public void Resolve_UndefinedMethod_Returns400ErrorHandler_AndSetsStatusCode()
    {
        var router = SetupRouter();
        var context = new MockHttpContext { Request = new MockHttpRequest { Method = Method.UNDEFINED, Path = "/api/users" } };

        var handler = router.Resolve(context);

        Assert.IsType<DummyErrorHandler>(handler);
        Assert.Equal(400, context.Response.StatusCode);
    }
}