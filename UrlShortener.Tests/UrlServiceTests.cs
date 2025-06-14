using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Data;
using UrlShortenerApi.Helpers;
using UrlShortenerApi.Models;
using UrlShortenerApi.Services;

namespace UrlShortener.Tests;

public class UrlServiceTests
{
    private readonly UrlService _service;
    private readonly ApplicationDbContext _context;

    public UrlServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "UrlDb_" + Guid.NewGuid())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new UrlService(_context);
    }

    [Fact]
    public void ShortenUrl_Returns_Shortened()
    {
        //Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = "https://www.google.com",
            ExpiresInDays = 7
        };
        var query = new ShortenRequestQuery();
        
        //Act
        var result = _service.ShortenUrl(request, query);
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Status);
        Assert.StartsWith("http://localhost:8080/", result.Url);
    }

    [Fact]
    public void ShortenUrl_Returns_CustomUrl()
    {
        //Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = "https://www.google.com",
            ExpiresInDays = 7
        };
        var query = new ShortenRequestQuery
        {
            CustomUrl = "my-cool-link"
        };
        
        //Act
        var result = _service.ShortenUrl(request, query);
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Status);
        Assert.Equal("http://localhost:8080/my-cool-link", result.Url);
    }

    [Fact]
    public void ShortenUrl_Returns_ConflictAlreadyExist()
    {
        //Arrange
        var exists = new UrlMapping
        {
            OriginalUrl = "https://www.facebook.com",
            ShortCode = "my-cool-link",
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(7),
        };
        _context.UrlMappings.Add(exists);
        _context.SaveChanges();
        
        var request = new ShortenRequest
        {
            OriginalUrl = "https://www.google.com",
            ExpiresInDays = 7
        };
        var query = new ShortenRequestQuery
        {
            CustomUrl = "my-cool-link"
        };
        
        //Act
        var result = _service.ShortenUrl(request, query);
        
        //Assert
        Assert.NotNull(result);
        Assert.False(result.Status);
        Assert.Equal("Custom short URL is already in use.", result.ErrorMessage);
    }

    [Fact]
    public void ShortenUrl_Returns_ShortWhenOriginalUrlExists()
    {
        //Arrange
        var exists = new UrlMapping
        {
            OriginalUrl = "https://www.facebook.com",
            ShortCode = "my-cool-link",
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(7),
        };
        _context.UrlMappings.Add(exists);
        _context.SaveChanges();

        var request = new ShortenRequest
        {
            OriginalUrl = "https://www.facebook.com",
            ExpiresInDays = 7
        };
        var query = new ShortenRequestQuery();
        
        //Act
        var result = _service.ShortenUrl(request, query);
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Status);
        Assert.Equal("http://localhost:8080/my-cool-link", result.Url);
    }

    [Fact]
    public void ShortenUrl_Returns_SameUrlTwice()
    {
        //Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = "https://www.facebook.com",
            ExpiresInDays = 7
        };
        var query = new ShortenRequestQuery();
        
        //Act
        var firstResult = _service.ShortenUrl(request, query);
        var secondResult = _service.ShortenUrl(request, query);
        
        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.True(firstResult.Status);
        Assert.Equal(firstResult.Url, secondResult.Url);
    }

    [Fact]
    public void ShortenUrl_Default7Days_WhenExpiryIsNull()
    {
        //Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = "https://www.facebook.com",
        };
        var query = new ShortenRequestQuery();
        
        //Act
        var result = _service.ShortenUrl(request, query);
        
        //Assert
        var mapping = _context.UrlMappings.FirstOrDefault(m => m.OriginalUrl == request.OriginalUrl);
        var expiry = mapping.CreatedOn.AddDays(7);
        
        Assert.Equal(expiry.Date, mapping.ExpiresOn.Date);
    }
    
    [Fact]
    public void GetOriginalUrl_Returns_OriginalUrl()
    {
        //Arrange
        string shortCode = "abc123";
        string originalUrl = "https://www.facebook.com";
        _context.UrlMappings.Add(new UrlMapping
        {
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(7),
        });
        _context.SaveChanges();
        
        //Act
        var result = _service.GetOriginalUrl(shortCode);
        
        //Assert
        Assert.NotNull(result);
        Assert.False(result.Expired);
        Assert.Equal(originalUrl, result.OriginalUrl);
    }

    [Fact]
    public void GetOriginalUrl_Returns_Expired()
    {
        string shortCode = "abc123";
        _context.UrlMappings.Add(new UrlMapping
        {
            OriginalUrl = "https://expired.com",
            ShortCode = shortCode,
            CreatedOn = DateTime.Now.AddDays(-8),
            ExpiresOn = DateTime.Now.AddDays(-1)
        });
        _context.SaveChanges();
        
        //Act
        var result = _service.GetOriginalUrl(shortCode);
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Expired);
    }

    [Fact]
    public void GetOriginalUrl_IncreaseClickedOnCount()
    {
        //Arrange
        string shortCode = "abc123";
        _context.UrlMappings.Add(new UrlMapping
        {
            OriginalUrl = "https://www.facebook.com",
            ShortCode = shortCode,
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(7),
            ClickedOn = 0
        });
        _context.SaveChanges();
        
        //Act
        _service.GetOriginalUrl(shortCode);
        
        //Assert
        var mapping = _context.UrlMappings.FirstOrDefault(m => m.OriginalUrl == "https://www.facebook.com");
        Assert.Equal(1, mapping.ClickedOn);
    }

    [Fact]
    public void GenerateCode_ChecksForConflicts()
    {
        //Arrange
        string shortCode = "MmsPgD"; //the actual code for https://massgrave.dev/
        _context.UrlMappings.Add(new UrlMapping
        {
            OriginalUrl = "https://www.facebook.com",
            ShortCode = shortCode,
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.AddDays(7),
        });
        _context.SaveChanges();
        
        var result = _service.GenerateCode("https://massgrave.dev/");
        
        Assert.NotNull(result);
        Assert.NotEqual(shortCode, result);
    }
}