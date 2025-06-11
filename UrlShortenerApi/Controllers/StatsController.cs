using Microsoft.AspNetCore.Mvc;
using UrlShortenerApi.Interfaces;

namespace UrlShortenerApi.Controllers;

[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    [HttpGet("{shortcode}")]
    public IActionResult Get([FromRoute] string shortcode)
    {
        var totalClicks = _statsService.TotalClicks(shortcode);
        if (totalClicks == null)
        {
            return NotFound("Short code not found");
        }
        return Ok(totalClicks);
    }
}