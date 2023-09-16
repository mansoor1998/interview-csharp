namespace UrlShortenerService.Application.Common.Exceptions;

public class ZeroLengthException : Exception
{
    public ZeroLengthException()
        : base()
    {
    }

    public ZeroLengthException(string message)
        : base(message)
    {
    }

    public ZeroLengthException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
