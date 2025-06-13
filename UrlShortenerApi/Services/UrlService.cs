using System.Security.Cryptography;
using System.Text;
using Base62;
using UrlShortenerApi.Data;
using UrlShortenerApi.Interfaces;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Services;

public class UrlService : IUrlService
{
    private readonly string _baseUrl = "http://localhost:8080/";
    private readonly ApplicationDbContext _context;

    public UrlService(ApplicationDbContext context)
    {
        _context = context;
    }
    public string ShortenUrl(string originalUrl)
    {
        var existing = _context.UrlMappings.FirstOrDefault(u => u.OriginalUrl == originalUrl);
        if (existing != null)
        {
            return _baseUrl + existing.ShortCode;
        }
        var shortenedUrl = GenerateCode(originalUrl);
        
        _context.UrlMappings.Add(new UrlMapping
        {
            OriginalUrl = originalUrl,
            ShortCode   = shortenedUrl,
        });
        _context.SaveChanges();
        
        return _baseUrl + shortenedUrl;
    }

    public string? GetOriginalUrl(string shortCode)
    {
        var entry = _context.UrlMappings.FirstOrDefault(u => u.ShortCode == shortCode);
        if (entry == null)
        {
            return null;
        }
        entry.ClickedOn++;
        _context.SaveChanges();
        return entry.OriginalUrl;
    }

    public string GenerateCode(string originalUrl)
    {
        SHA256 sha256 = SHA256.Create();
        string shortCode;
        int counter = 0;

        do
        {
            string input = counter == 0 ? originalUrl : originalUrl + counter.ToString();
            byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            var dataBase62 = data.ToBase62();
            shortCode = dataBase62.Substring(0, 6);
            counter++;
        }
        while (_context.UrlMappings.Any(u => u.ShortCode == shortCode));

        return shortCode;
    }

}