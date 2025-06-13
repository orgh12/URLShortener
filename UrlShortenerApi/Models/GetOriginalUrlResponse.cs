namespace UrlShortenerApi.Models;

public class GetOriginalUrlResponse
{
    public required string OriginalUrl { get; set; }
    public bool Expired { get; set; }
}