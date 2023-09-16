using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Url.Commands;

public record CreateShortUrlCommand : IRequest<string>
{
    public string Url { get; init; } = default!;
}

public class CreateShortUrlCommandValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlCommandValidator()
    {
        _ = RuleFor(v => v.Url)
          .NotEmpty()
          .WithMessage("Url is required.");
    }
}

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;

    public CreateShortUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public async Task<string> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        var reuqestUrl = request.Url;
        var urlEntity = await _context.Urls.FirstOrDefaultAsync(url => url.OriginalUrl == reuqestUrl);

        string encoded;

        if (urlEntity != null)
        {
            var entityId = urlEntity.Id;
            encoded = _hashids.Encode((int) entityId);

            return GetUrl() + encoded;
        }

        var urlInstance = new Domain.Entities.Url
        {
            OriginalUrl = reuqestUrl
        };

        _ = _context.Urls.Add(urlInstance);
        _ = await _context.SaveChangesAsync(cancellationToken);

        var id = urlInstance.Id;

        encoded = _hashids.EncodeLong(id);

        return GetUrl() + encoded;
    }

    private string GetUrl()
    {
        return "http://localhost:5246/u/";
    }
}
