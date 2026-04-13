namespace IzTek.Carbon.Footprint.Domain.Common.Exceptions;

public class DomainException : ApplicationException
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}