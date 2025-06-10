using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortenerApi.Controllers;
using UrlShortenerApi.Interfaces;
using UrlShortenerApi.Models;

namespace UrlShortener.Tests;

public class UrlControllerTests
{
    [Theory]
    [InlineData( "http://google.com", "http://localhost:8080/abc123")]
    public void PostUrl_Returns_Shortened(string originalUrl, string expected)
    {
        //Arrange
        var urlService = new Mock<IUrlService>();
        urlService.Setup(u => u.ShortenUrl(originalUrl)).Returns(expected);
        
        var request = new ShortenRequest()
        {
            OriginalUrl = originalUrl,
        };
        var controller = new UrlController(urlService.Object);
        
        //Act
        var result = controller.Post(request);
        
        //Assert
        var okObjectResult = result as OkObjectResult;
        Assert.NotNull(okObjectResult);
        Assert.Equal(expected, okObjectResult.Value);
    }

    [Fact]
    public void PostEmptyUrl_Returns_BadRequest()
    {
        //Arrange
        var urlService = new Mock<IUrlService>();
        var request = new ShortenRequest()
        {
            OriginalUrl = "",
        };
        var controller = new UrlController(urlService.Object);
        
        //Act
        var result = controller.Post(request);
        
        //Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.NotNull(badRequest);
        Assert.Equal("Original URL cannot be empty", badRequest.Value);
    }

    [Fact]
    public void PostNullObject_Returns_BadRequest()
    {
        //Arrange
        var urlService = new Mock<IUrlService>();
        var request = null as ShortenRequest;
        var controller = new UrlController(urlService.Object);
        
        //Act
        var result = controller.Post(request);
        
        //Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.NotNull(badRequest);
        Assert.Equal("Request object is null", badRequest.Value);
    }
}