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
        var response = _urlService.GetOriginalUrl(shortCode);
        if (response == null)
        {
            return NotFound();
        }

        if (response.Expired)
        {
            return BadRequest("This Url has Expired");
        }
        return Redirect(response.OriginalUrl);
    }
}