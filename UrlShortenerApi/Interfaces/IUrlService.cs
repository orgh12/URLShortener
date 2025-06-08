namespace UrlShortenerApi.Interfaces;

public interface IUrlService
{
    public string ShortenUrl(string originalUrl);
    public string GetOriginalUrl(string shortCode);
    public string GenerateCode(string shortCode);
}