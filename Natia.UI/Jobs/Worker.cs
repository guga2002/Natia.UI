namespace Natia.UI.Jobs;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker service started at: {time}", DateTime.UtcNow);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogDebug("Worker loop triggered at: {time}", DateTime.UtcNow);

                using (var scope = _scopeFactory.CreateScope())
                {
                    var mainService = scope.ServiceProvider.GetRequiredService<NatiaGuard.BrainStorm.Main.Main>();

                    _logger.LogInformation("Executing NatiaGuard Main.Start at: {time}", DateTime.UtcNow);
                    await mainService.Start();
                    _logger.LogInformation("Completed NatiaGuard Main.Start at: {time}", DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Worker loop at: {time}", DateTime.UtcNow);
            }

            await Task.Delay(500, stoppingToken);
        }

        _logger.LogInformation("Worker service stopped at: {time}", DateTime.UtcNow);
    }
}
