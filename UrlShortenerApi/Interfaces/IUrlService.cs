using UrlShortenerApi.Helpers;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Interfaces;

public interface IUrlService
{
    public string ShortenUrl(ShortenRequest originalUrl, ShortenRequestQuery query);
    public GetOriginalUrlResponse? GetOriginalUrl(string shortCode);
    public string GenerateCode(string shortCode);
}