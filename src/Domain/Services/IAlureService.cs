namespace AluraCrawler.Domain.Services
{
    public interface IAlureService
    {
        Task Run(CancellationToken stoppingToken);
    }
}