namespace UrlShortenerApi.Models;

public class ShortenResponse
{
    public string? Url { get; set; }
    public bool Status { get; set; }
    public string? ErrorMessage { get; set; }
}