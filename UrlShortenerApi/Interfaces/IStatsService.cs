namespace UrlShortenerApi.Interfaces;

public interface IStatsService
{
    public int? TotalClicks(string shortCode);
}