using Microsoft.AspNetCore.Mvc;
using UrlShortenerApi.Interfaces;
using UrlShortenerApi.Models;
using UrlShortenerApi.Services;

namespace UrlShortenerApi.Controllers;

[Route("api/shorten")]
[ApiController]
public class UrlController : ControllerBase

{
    private readonly IUrlService _urlService;

    public UrlController(IUrlService urlService)
    {
        _urlService = urlService;
    }
    [HttpPost]
    public IActionResult Post([FromBody] ShortenRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }
        string shortenedUrl = _urlService.ShortenUrl(request.OriginalUrl);
        return Ok(shortenedUrl);
    }
}