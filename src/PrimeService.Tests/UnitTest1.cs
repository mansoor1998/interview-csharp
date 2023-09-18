using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using UrlShortenerService.Application.Common.Interfaces;
using UrlShortenerService.Application.Url.Commands;
using UrlShortenerService.Domain.Entities;

namespace PrimeService.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Handler_ShouldRedirectToRequestedUrl()
    {
        // Arrange
        string encodedId = "123asdf";
        long Id = 1;
        string url = "http://www.example.com";

        var _contextMock = new Mock<IApplicationDbContext>();
        var _hashIdMock = new Mock<IHashids>();
        var _dbSetUrlMock = new Mock<DbSet<Url>>();

        var urlEntity = new Url()
        {
            Id = Id,
            OriginalUrl = url
        };

        var urlEntities = new List<Url>() { urlEntity, new Url() { Id = 2, OriginalUrl = "http://www.google.com" } };

        _hashIdMock.Setup(x => x.DecodeSingleLong(encodedId)).Returns(Id);
        _contextMock.Setup(x => x.Urls).ReturnsDbSet(urlEntities);
        // _contextMock.Setup(x => x.Urls).Returns(_dbSetUrlMock.Object);
        // _dbSetUrlMock.Setup(x => x.)
        var command = new RedirectToUrlCommand()
        {
            Id = encodedId
        };
        var handler = new RedirectToUrlCommandHandler(_contextMock.Object, _hashIdMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.Equal(result, url);
    }

    [Fact]
    public async Task Handler_ShouldReturnSameUrlOnCreate()
    {
        // Arrange
        const string EncodedId = "123asdf";
        const long Id = 1;
        const string Url = "http://www.example.com";

        var _contextMock = new Mock<IApplicationDbContext>();
        var _hashIdMock = new Mock<IHashids>();
        var _dbSetUrlMock = new Mock<DbSet<Url>>();

        var urlEntity = new Url()
        {
            Id = Id,
            OriginalUrl = Url
        };

        var urlEntities = new List<Url>() { urlEntity, new Url() { Id = 2, OriginalUrl = "http://www.google.com" } };

        _hashIdMock.Setup(x => x.EncodeLong(Id)).Returns(EncodedId);
        _contextMock.Setup(x => x.Urls).ReturnsDbSet(urlEntities);
        
        var command = new CreateShortUrlCommand()
        {
            Url = "http://www.example.com"
        };
        var handler = new CreateShortUrlCommandHandler(_contextMock.Object, _hashIdMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);
        
        
        Assert.Equal(result, "http://localhost:5246/u/" + EncodedId);
    }
}
