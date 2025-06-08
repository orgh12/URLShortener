using Microsoft.AspNetCore.Mvc;
using UrlShortenerApi.Interfaces;

namespace UrlShortenerApi.Controllers;

[Route("")]
[ApiController]
public class RedirectController : ControllerBase
{
    private readonly IUrlService _urlService;

    public RedirectController(IUrlService urlService)
    {
        _urlService = urlService;
    }

    [HttpGet("{shortCode}")]
    public IActionResult Get([FromRoute] string shortCode)
    {
        var originalUrl = _urlService.GetOriginalUrl(shortCode);
        if (originalUrl == null)
        {
            return NotFound();
        }
        return Redirect(originalUrl);
    }
}