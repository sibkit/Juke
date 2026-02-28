/* Program.cs */
using Flux.Host.Handlers;
using Flux.Host.Services;
using Flux.Pcb;
using Juke;
using Juke.Web.AspNetCore;
using Juke.Web.AspNetCore.Http;
using Juke.Web.Core.Assets;
using Juke.Web.Core.Handlers;
using Juke.Web.Core.Http;
using Juke.Web.Core.Render;
using Juke.Web.Core.Routing;
using Juke.Web.Core.WebSockets;

namespace Flux.Host;

public class Program {
    public static void Main(string[] args) {
        // 1. Инициализация нашего кастомного DI-контейнера (Juke.Locator)
        Locator.AddSingleton<AssetRegistry>(loc => new AssetRegistry());
        Locator.AddSingleton<AppWebSocketHub>(loc => new AppWebSocketHub());
        Locator.AddSingleton<ILayoutFactory>(loc => new HostLayoutFactory());
        Locator.Freeze();

        var builder = WebApplication.CreateBuilder(args);

        // 2. Мост для ASP.NET Core (чтобы HostedService мог получить доступ к нашему хабу)
        builder.Services.AddSingleton(sp => Locator.Get<AppWebSocketHub>());
        builder.Services.AddHostedService<SimulationBackgroundService>();

        var app = builder.Build();

        app.UseWebSockets();
        app.UseRouting();

#pragma warning disable ASP0014
        app.UseEndpoints(endpoints => {
            endpoints.Map("/ws/app", async context => {
                // Создаем Scope и передаем его в наш адаптер для WebSockets
                using var scope = Locator.CreateScope();
                scope.Fallback = new AspServiceProviderWrapper(context.RequestServices);

                var jukeContext = new AspContextAdapter(context, scope);
                var handler = new AppWebSocketHandler();
                await handler.HandleAsync(jukeContext);
            });
        });

        // 3. Сборка дерева роутов
        var rootNode = new GroupRouteNode();
        rootNode.AddHandler(Method.GET, new HomeHandler()); // Главная

        // ИСПРАВЛЕНИЕ: Подключаем модуль целиком как ветку!
        // Теперь все дочерние узлы (включая raw/{fileName}) автоматически подхватятся.
        var pcbModule = new FluxPcbModule();
        rootNode.Mount("pcb", pcbModule.GetModuleRoute()); 

        var router = new Router(rootNode);
        var assetHandler = new AssetHandler();

        // 4. Главный пайплайн обработки HTTP-запросов
        app.Run(async (context) => {
            // Формируем жизненный цикл (Scope) зависимостей на 1 запрос
            using var scope = Locator.CreateScope();
            scope.Fallback = new AspServiceProviderWrapper(context.RequestServices);

            var jukeContext = new AspContextAdapter(context, scope);

            // Перехватываем запросы к статике
            if (jukeContext.Request.Path.StartsWith("/assets/", StringComparison.OrdinalIgnoreCase)) {
                await assetHandler.HandleAsync(jukeContext);
                return;
            }

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