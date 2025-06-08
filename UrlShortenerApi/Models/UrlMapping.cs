using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApi.Models;

public class UrlMapping
{
    public int Id { get; set; }
    public string ShortCode { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}