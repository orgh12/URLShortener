using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortenerApi.Controllers;
using UrlShortenerApi.Helpers;
using UrlShortenerApi.Interfaces;
using UrlShortenerApi.Models;

namespace UrlShortener.Tests;

public class UrlControllerTests
{
    private readonly Mock<IUrlService> _urlService;
    private readonly UrlController _urlController;

    public UrlControllerTests()
    {
        _urlService = new Mock<IUrlService>();
        _urlController = new UrlController(_urlService.Object);
    }
    
    [Theory]
    [InlineData( "http://google.com", "http://localhost:8080/abc123")]
    public void PostUrl_Returns_Shortened(string originalUrl, string expected)
    {
        //Arrange
        var query = new ShortenRequestQuery();
        var request = new ShortenRequest()
        {
            OriginalUrl = originalUrl,
            ExpiresInDays = 4
        };
        var response = new ShortenResponse
        {
            Status = true,
            Url = expected,
        };
        _urlService.Setup(x => x.ShortenUrl(request, query)).Returns(response);
        
        //Act
        var result = _urlController.Post(request, query);
        
        //Assert
        var okObjectResult = result as OkObjectResult;
        Assert.NotNull(okObjectResult);
        Assert.Equal(expected, okObjectResult.Value);
    }

    [Fact]
    public void PostEmptyUrl_Returns_BadRequest()
    {
        //Arrange
        var request = new ShortenRequest()
        {
            OriginalUrl = "",
            ExpiresInDays = 4
        };
        
        //Act
        var result = _urlController.Post(request, new ShortenRequestQuery());
        
        //Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.NotNull(badRequest);
        Assert.Equal("Original URL cannot be empty", badRequest.Value);
    }

    [Theory]
    [InlineData( "http://google.com", "http://localhost:8080/abc123")]
    public void PostCustomUrl_Returns_BadRequestTooLong(string originalUrl, string expected)
    {
        //Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = originalUrl,
            ExpiresInDays = 4
        };
        var query = new ShortenRequestQuery
        {
            CustomUrl = new string('a',21)
        };
        
        //Act
        var result = _urlController.Post(request, query);
        
        //Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.NotNull(badRequest);
        Assert.Equal("Custom URL cannot be longer than 20 characters", badRequest.Value);
    }

    [Theory]
    [InlineData("http://google.com", "my-cool-link")]
    public void PostCustomUrl_Returns_Ok(string originalUrl, string expected)
    {
        //Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = originalUrl,
            ExpiresInDays = 4
        };
        var query = new ShortenRequestQuery
        {
            CustomUrl = "my-cool-link"
        };
        _urlService.Setup(x => x.ShortenUrl(request, query)).Returns(new ShortenResponse { Status = true, Url = expected});
        
        //Act
        var result = _urlController.Post(request, query);
        
        //Assert
        var okObjectResult = result as OkObjectResult;
        Assert.NotNull(okObjectResult);
        Assert.Equal(expected, okObjectResult.Value);
    }

    [Theory]
    [InlineData( "http://google.com", "my-cool-link")]
    public void PostCustomUrl_Returns_Conflict(string originalUrl, string customUrl)
    {
        //Arrange
        var request = new ShortenRequest
        {
            OriginalUrl = originalUrl,
            ExpiresInDays = 4
        };
        var query = new ShortenRequestQuery
        {
            CustomUrl = customUrl
        };

        //Act
        _urlService.Setup(u => u.ShortenUrl(request, query)).Returns(new ShortenResponse { Status = false, ErrorMessage = "Custom short URL is already in use."});
        var result = _urlController.Post(request, query);
        
        //Assert
        var conflictObjectResult = result as ConflictObjectResult;
        Assert.NotNull(conflictObjectResult);
        Assert.Equal("Custom short URL is already in use.", conflictObjectResult.Value);
    }
}