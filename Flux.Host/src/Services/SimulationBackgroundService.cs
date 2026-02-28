/* Flux.Host/Services/SimulationBackgroundService.cs */

using Juke.Web.Core.WebSockets;

namespace Flux.Host.Services;

public class SimulationBackgroundService : BackgroundService
{
    private readonly AppWebSocketHub _hub;
    private decimal _salesToday = 15400.50m;

    public SimulationBackgroundService(AppWebSocketHub hub)
    {
        _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Бесконечный цикл, пока приложение не остановят
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;

            // 1. Рассылаем серверное время в канал футера
            await _hub.BroadcastAsync("server-time", new { time = now.ToString("HH:mm:ss") });

            // 2. Логика обнуления в 00:00:00
            if (now.Hour == 0 && now.Minute == 0 && now.Second == 0)
            {
                _salesToday = 0m;
            }
            else
            {
                // Имитируем случайные покупки (от 0 до 5 долларов) каждую секунду
                var randomPurchase = (decimal)(new Random().NextDouble() * 5.0);
                _salesToday += randomPurchase;
            }

            // 3. Рассылаем новые продажи в канал дашборда
            // Важно: канал должен совпадать с тем, что в DashboardContent ("sales-main-branch")
            await _hub.BroadcastAsync("sales-main-branch", new { newTotal = _salesToday.ToString("F2") });

            // Ждем ровно 1 секунду
            await Task.Delay(250, stoppingToken);
        }
    }
}