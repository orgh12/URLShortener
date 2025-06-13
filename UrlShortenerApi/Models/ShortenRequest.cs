namespace UrlShortenerApi.Models;

public class ShortenRequest
{
    public string OriginalUrl { get; set; }
    public int? ExpiresInDays { get; set; }
}