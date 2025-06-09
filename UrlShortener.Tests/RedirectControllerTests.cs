using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortenerApi.Controllers;
using UrlShortenerApi.Interfaces;
using UrlShortenerApi.Services;

namespace UrlShortener.Tests;

public class RedirectControllerTests
{
    [Fact]
    public void GetShortCode_Returns_Redirect()
    {
        
        //Arrange
        var shortCode = "a78db0";
        var originalUrl =
            "https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio";
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
}