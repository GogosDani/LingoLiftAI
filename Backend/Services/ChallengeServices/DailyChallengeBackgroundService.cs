namespace Backend.Services.ChallengeServices;

public class DailyChallengeBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyChallengeBackgroundService> _logger;

    public DailyChallengeBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<DailyChallengeBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var challengeGenerator = scope.ServiceProvider.GetRequiredService<IDailyChallengeGeneratorService>();
                
                await challengeGenerator.EnsureTodaysChallengeExistsAsync();
                
                var tomorrow = DateTime.Today.AddDays(1);
                await challengeGenerator.GenerateChallengeForDateAsync(tomorrow);
                
                _logger.LogInformation("Daily challenge check completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in daily challenge background service");
            }
            
            var nextRun = DateTime.Today.AddDays(1).AddHours(0).AddMinutes(5);
            var delay = nextRun - DateTime.Now;
            
            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay, stoppingToken);
            }
            else
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}