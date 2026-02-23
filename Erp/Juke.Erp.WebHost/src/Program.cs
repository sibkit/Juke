using Juke.Erp.WebHost.Handlers;
using Juke.Web.AspNetCore;
using Juke.Web.Core;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Routing;

namespace Juke.Erp.WebHost;

public class Program 
{
    public static void Main(string[] args) 
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        // 1. Build the Route Tree directly in memory
        var rootNode = new GroupRouteNode();
        
        rootNode.AddHandler(Method.GET, new HomeHandler());

        var router = new Router(rootNode) {
            ErrorHandlers = {
                // Register Error Handlers
                [404] = new FallbackErrorHandler(404, "Page Not Found"),
                [400] = new FallbackErrorHandler(400, "Bad Request")
            }
        };

        // 2. Connect ASP.NET Core pipeline directly to the Router instance via closure
        app.Run(async (context) => 
        {
            var jukeContext = new ContextAdapter(context);
            
            // We use the 'router' variable directly from the outer scope
            var handler = router.Resolve(jukeContext);
            
            switch (handler) {
                case IRequestHandler requestHandler:
                    await requestHandler.HandleAsync(jukeContext);
                    break;
                case IErrorHandler errorHandler:
                    await errorHandler.HandleAsync(jukeContext, null);
                    break;
                default:
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Internal Server Error: No handler resolved.");
                    break;
            }
        });

        app.Run();
    }
}

// --- Handlers ---

public class HelloWorldHandler : IRequestHandler 
{
    public async Task HandleAsync(IHttpContext context) 
    {
        context.Response.SetContentType("text/html; charset=utf-8");
        await context.Response.WriteAsync("<h1>Juke ERP System is Running (No DI for Router)!</h1>");
    }
}

public class FallbackErrorHandler : IErrorHandler 
{
    private readonly int _statusCode;
    private readonly string _message;

    public FallbackErrorHandler(int statusCode, string message) 
    {
        _statusCode = statusCode;
        _message = message;
    }

    public async Task HandleAsync(IHttpContext context, Exception? exception) 
    {
        context.Response.StatusCode = _statusCode;
        context.Response.SetContentType("text/plain; charset=utf-8");
        await context.Response.WriteAsync($"Error {_statusCode}: {_message}");
    }
}