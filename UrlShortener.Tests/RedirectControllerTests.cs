using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortenerApi.Controllers;
using UrlShortenerApi.Interfaces;
using UrlShortenerApi.Services;

namespace UrlShortener.Tests;

public class RedirectControllerTests
{
    [Theory]
    [InlineData("a78db0","https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio")]
    [InlineData("a44ffd","https://example.com")]
    public void GetShortCode_Returns_Redirect(string shortCode, string originalUrl)
    {
        
        //Arrange
        var urlServiceMock = new Mock<IUrlService>();
        urlServiceMock.Setup(u => u.GetOriginalUrl(shortCode)).Returns(originalUrl);
        
        var controller = new RedirectController(urlServiceMock.Object);
        
        //Act
        var result = controller.Get(shortCode);
        
        //Assert
        var redirectResult = result as RedirectResult;
        Assert.NotNull(redirectResult);
        Assert.Equal(originalUrl, redirectResult.Url);
    }

    [Theory]
    [InlineData("yd8daw")]
    public void GetShortCode_Returns_NotFound(string shortCode)
    {
        //Arrange
        var urlServiceMock = new Mock<IUrlService>();
        urlServiceMock.Setup(u => u.GetOriginalUrl(shortCode)).Returns(() => null);
        var controller = new RedirectController(urlServiceMock.Object);
        
        //Act
        var result = controller.Get(shortCode);
        
        //Assert
        var notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
    }
}