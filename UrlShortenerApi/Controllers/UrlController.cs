using Microsoft.AspNetCore.Mvc;
using UrlShortenerApi.Helpers;
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
    public IActionResult Post([FromBody] ShortenRequest request, [FromQuery] ShortenRequestQuery query)
    {
        if (request.OriginalUrl == "")
        {
            return BadRequest("Original URL cannot be empty");
        }

        if (query.CustomUrl is { Length: > 20 })
        {
            return BadRequest("Custom URL cannot be longer than 20 characters");
        }
        string shortenedUrl = _urlService.ShortenUrl(request, query);
        return Ok(shortenedUrl);
    }
}