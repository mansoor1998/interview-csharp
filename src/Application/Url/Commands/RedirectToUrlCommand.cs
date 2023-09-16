using System;
using FluentValidation;
using HashidsNet;
using MediatR;
using UrlShortenerService.Application.Common.Exceptions;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Url.Commands;

public record RedirectToUrlCommand : IRequest<string>
{
    public string Id { get; init; } = default!;
}

public class RedirectToUrlCommandValidator : AbstractValidator<RedirectToUrlCommand>
{
    public RedirectToUrlCommandValidator()
    {
        _ = RuleFor(v => v.Id)
          .NotEmpty()
          .WithMessage("Id is required.");
    }
}

public class RedirectToUrlCommandHandler : IRequestHandler<RedirectToUrlCommand, string?>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;

    public RedirectToUrlCommandHandler(IApplicationDbContext context, IHashids hashids)
    {
        _context = context;
        _hashids = hashids;
    }

    public async Task<string?> Handle(RedirectToUrlCommand request, CancellationToken cancellationToken)
    {
        var decodedArr = _hashids.DecodeLong(request.Id);

        if (decodedArr.Length == 0)
        {
            throw new ZeroLengthException("Array cannot have length 0");
        }

        var url = _context.Urls.FirstOrDefault(url => url.Id == decodedArr[0]);
        return url?.OriginalUrl ?? null;
    }
}