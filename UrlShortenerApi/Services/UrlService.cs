using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
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
        return entry?.OriginalUrl;
    }

    public string GenerateCode(string shortCode)
    {
        SHA256 sha256 = SHA256.Create();
        byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(shortCode));
        var sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        
        return sBuilder.ToString().Substring(0,6);
    }
}