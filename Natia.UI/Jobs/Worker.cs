namespace Natia.UI.Jobs
{
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
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var mainService = scope.ServiceProvider.GetRequiredService<NatiaGuard.BrainStorm.Main.Main>();

                    await mainService.Start();
                }
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
