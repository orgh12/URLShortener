namespace UrlShortenerApi.Models;

public class ShortenRequest
{
    public string OriginalUrl { get; set; }
    public double? ExpiresInDays { get; set; }
}