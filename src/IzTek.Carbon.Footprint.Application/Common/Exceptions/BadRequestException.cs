namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class BadRequestException: ApplicationException
{
    public BadRequestException(): base("Bad request")
    {

    }

    public BadRequestException(string message) : base(message)
    {
    
    }

    public BadRequestException(string message, Exception innerException) : base(message, innerException)
    {

    }
}