using System.ComponentModel.DataAnnotations;

namespace UrlShortenerApi.Models;

public class UrlMapping
{
    public int Id { get; set; }
    public string ShortCode { get; set; }
    public string OriginalUrl { get; set; }
    public int ClickedOn { get; set; } = 0;
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}