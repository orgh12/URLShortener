using System.Security.Cryptography;
using System.Text;
using Base62;
using Microsoft.IdentityModel.Tokens;
using UrlShortenerApi.Data;
using UrlShortenerApi.Helpers;
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
    public ShortenResponse ShortenUrl(ShortenRequest request, ShortenRequestQuery query)
    {
        var hasCustom = !string.IsNullOrWhiteSpace(query.CustomUrl);
        
        var existing = _context.UrlMappings
            .FirstOrDefault(u => 
                u.OriginalUrl == request.OriginalUrl || 
                (hasCustom && u.ShortCode == query.CustomUrl));
        
        if (existing != null && existing.OriginalUrl == request.OriginalUrl)
        {
            if (request.ExpiresInDays != null)
            {
                existing.ExpiresOn = DateTime.Now.AddDays(request.ExpiresInDays.Value);
                _context.SaveChanges();
            }
            
            return new ShortenResponse
            {
                Status = true,
                Url = _baseUrl + existing.ShortCode,
            };
        }
        
        if (hasCustom && existing != null && existing.ShortCode == query.CustomUrl)
        {
            return new ShortenResponse()
            {
                Status = false,
                ErrorMessage = "Custom short URL is already in use."
            };
        }
        
        string shortCode = hasCustom
            ? query.CustomUrl!
            : GenerateCode(request.OriginalUrl);

        _context.UrlMappings.Add(new UrlMapping
        {
            OriginalUrl = request.OriginalUrl,
            ShortCode = shortCode,
            CreatedOn = DateTime.Now,
            ExpiresOn = request.ExpiresInDays == null
                ? DateTime.Now.AddDays(7)
                : DateTime.Now.AddDays(request.ExpiresInDays.Value)
        });

        _context.SaveChanges();
        
        return new ShortenResponse()
        {
            Status = true,
            Url = _baseUrl + shortCode
        };
    }

    public GetOriginalUrlResponse? GetOriginalUrl(string shortCode)
    {
        var entry = _context.UrlMappings.FirstOrDefault(u => u.ShortCode == shortCode);
        if (entry == null)
        {
            return null;
        }

        if (DateTime.Compare(entry.ExpiresOn, DateTime.Now) <= 0)
        {
            return new GetOriginalUrlResponse
            {
                Expired = true,
                OriginalUrl = ""
            };
        }
        entry.ClickedOn++;
        _context.SaveChanges();
        return new GetOriginalUrlResponse
        {
            Expired = false,
            OriginalUrl = entry.OriginalUrl,
        };
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