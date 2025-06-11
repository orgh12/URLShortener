using UrlShortenerApi.Data;
using UrlShortenerApi.Interfaces;

namespace UrlShortenerApi.Services;

public class StatsService : IStatsService
{
    private readonly ApplicationDbContext _context;

    public StatsService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public int? TotalClicks(string shortCode)
    {
        var shortCodeObject = _context.UrlMappings.FirstOrDefault(u => u.ShortCode == shortCode);
        if (shortCodeObject == null)
        {
            return null;
        }

        return shortCodeObject.ClickedOn;
    }
}