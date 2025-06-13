using UrlShortenerApi.Models;

namespace UrlShortenerApi.Interfaces;

public interface IUrlService
{
    public string ShortenUrl(ShortenRequest originalUrl);
    public string? GetOriginalUrl(string shortCode);
    public string GenerateCode(string shortCode);
}