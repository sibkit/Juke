using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Juke.Web.Core;
using Juke.Web.Core.Routing;

namespace Juke.Web.Tests;

// --- Заглушки для тестов ---
public class DummyHandler : IRequestHandler { 
    public string Name { get; }
    public DummyHandler(string name = "") { Name = name; }
    public void Handle(IHttpContext context) {} 
}

public class DummyErrorHandler : IErrorHandler {
    public void Handle(IHttpContext context, Exception? exception) {}
}

public class AnyStringMatcher : IPathPartMatcher {
    public bool TryMatch(ReadOnlySpan<char> pathPart, out object? parsedValue) {
        if (pathPart.IsEmpty) {
            parsedValue = null;
            return false;
        }
        parsedValue = pathPart.ToString();
        return true;
    }
}

public class MockHttpRequest : IHttpRequest {
    public Method Method { get; init; }
    public string Path { get; init; } = string.Empty;
    public string QueryString { get; } = string.Empty;
    public Dictionary<string, object> RouteValues { get; } = new();
    public IReadOnlyDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    public Stream Body { get; } = Stream.Null;
}

public class MockHttpResponse : IHttpResponse {
    public int StatusCode { get; set; } = 200;
    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    public Stream Body { get; } = Stream.Null;
}

public class MockHttpContext : IHttpContext {
    public IHttpRequest Request { get; set; } = new MockHttpRequest();
    public IHttpResponse Response { get; } = new MockHttpResponse();
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
        apiNode.Mount("v1", v1Group); // Монтируем группу по пути "v1"

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
        Assert.Equal(200, context.Response.StatusCode); // Статус не должен измениться
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