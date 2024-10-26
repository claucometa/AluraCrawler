using AluraCrawler.Domain.Services;

namespace AluraCrawler
{
    public class Worker(ILogger<Worker> logger, IAlureService service) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await service.Run(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }            
        }
    }
}
