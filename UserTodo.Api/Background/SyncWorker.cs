using UserTodo.Api.Services;

namespace UserTodo.Api.Background;

public class SyncWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;
    private readonly ILogger<SyncWorker> _logger;

    public SyncWorker(IServiceProvider services, IConfiguration config, ILogger<SyncWorker> logger)
    {
        _services = services;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = _config.GetValue("Sync:IntervalMinutes", 60);
        if (intervalMinutes <= 0)
            intervalMinutes = 60;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var sync = scope.ServiceProvider.GetRequiredService<SyncService>();
                await sync.SyncAsync(stoppingToken);
                _logger.LogInformation("Sync completed");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Sync failed");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
