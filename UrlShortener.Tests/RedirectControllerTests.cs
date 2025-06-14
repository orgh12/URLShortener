using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortenerApi.Controllers;
using UrlShortenerApi.Interfaces;
using UrlShortenerApi.Models;
using UrlShortenerApi.Services;

namespace UrlShortener.Tests;

public class RedirectControllerTests
{
    private readonly Mock<IUrlService> _urlService;
    private readonly RedirectController _redirectController;

    public RedirectControllerTests()
    {
        _urlService = new Mock<IUrlService>();
        _redirectController = new RedirectController(_urlService.Object);
    }
    
    [Theory]
    [InlineData("a78db0","https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio")]
    [InlineData("a44ffd","https://example.com")]
    public void GetOriginalUrl_Returns_Redirect(string shortCode, string originalUrl)
    {
        
        //Arrange
        var response = new GetOriginalUrlResponse
        {
            Expired = false,
            OriginalUrl = originalUrl,
        };
        _urlService.Setup(u => u.GetOriginalUrl(shortCode)).Returns(response);
        
        //Act
        var result = _redirectController.Get(shortCode);
        
        //Assert
        var redirectResult = result as RedirectResult;
        Assert.NotNull(redirectResult);
        Assert.Equal(originalUrl, redirectResult.Url);
    }

    [Theory]
    [InlineData("yd8daw")]
    public void GetOriginalUrl_Returns_NotFound(string shortCode)
    {
        //Arrange
        _urlService.Setup(u => u.GetOriginalUrl(shortCode)).Returns(() => null);
        
        //Act
        var result = _redirectController.Get(shortCode);
        
        //Assert
        var notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
    }

    [Theory]
    [InlineData("yd8daw")]
    public void GetShortCode_Returns_Expired(string shortCode)
    {
        //Arrange
        var response = new GetOriginalUrlResponse
        {
            Expired = true,
            OriginalUrl = ""
        };
        _urlService.Setup(u => u.GetOriginalUrl(shortCode)).Returns(response);
        
        //Act
        var result = _redirectController.Get(shortCode);
        
        //Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal("This Url has Expired", badRequestResult.Value);
    }
}