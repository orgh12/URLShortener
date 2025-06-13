using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApi.Models;

public class UrlMapping
{
    public int Id { get; set; }
    [MaxLength(8)]
    public required string ShortCode { get; set; }
    [MaxLength(2000)]
    public required string OriginalUrl { get; set; }
    public int ClickedOn { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}