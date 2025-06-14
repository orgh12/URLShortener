using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApi.Models;

public class ShortenRequest
{
    public required string OriginalUrl { get; set; }
    public double? ExpiresInDays { get; set; }
}