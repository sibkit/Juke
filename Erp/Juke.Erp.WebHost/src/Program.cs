using Juke.Erp.WebHost.Handlers;
using Juke.Web.AspNetCore.Http;
using Juke.Web.Core;
using Juke.Web.Core.Assets;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;
using Juke.Web.Core.Routing;
using Juke.Web.Core.WebSockets; 

namespace Juke.Erp.WebHost;

public interface IOp {
    string GetText();
}

public class Program 
{
    public static void Main(string[] args) 
    {
        
        
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddSingleton<AssetRegistry>();
        builder.Services.AddSingleton<AppWebSocketHub>(); 
        builder.Services.AddHostedService<Services.SimulationBackgroundService>();
        var app = builder.Build();
        
        app.UseWebSockets(); 
        app.UseRouting();
        
        #pragma warning disable ASP0014
        app.UseEndpoints(endpoints => 
        {
            endpoints.Map("/ws/app", async context => 
            {
                var jukeContext = new AspContextAdapter(context);
                var handler = new AppWebSocketHandler();
                await handler.HandleAsync(jukeContext);
            });
        });
        
        var rootNode = new GroupRouteNode();
        rootNode.AddHandler(Method.GET, new HomeHandler());
        
        var router = new Router(rootNode);
        var assetHandler = new AssetHandler();

        app.Run(async (context) => 
        {
            var jukeContext = new AspContextAdapter(context);
    
            if (jukeContext.Request.Path.StartsWith("/assets/", StringComparison.OrdinalIgnoreCase)) 
            {
                await assetHandler.HandleAsync(jukeContext);
                return;
            }
    
            // --- ЛОГИРУЕМ ВСЕ ЗАПРОСЫ К РОУТЕРУ ---
            Console.WriteLine($"[ROUTER] Пришел запрос: {jukeContext.Request.Method} {jukeContext.Request.Path}");
    
            var handler = router.Resolve(jukeContext);
    
            if (handler is IRequestHandler reqHandler) {
                Console.WriteLine($"[ROUTER] Найден обработчик: {reqHandler.GetType().Name}");
                await reqHandler.HandleAsync(jukeContext);
            } else {
                Console.WriteLine($"[ROUTER] Обработчик не найден! Возвращаем 404.");
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Not Found");
            }
        });

        app.Run();
    }
}