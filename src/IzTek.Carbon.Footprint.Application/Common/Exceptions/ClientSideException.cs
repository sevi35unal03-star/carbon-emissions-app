namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class ClientSideException : ApplicationException
{
    public ClientSideException() : base("A client-side error occurred.")
    {

    }

    public ClientSideException(string message) : base(message)
    {

    }

    public ClientSideException(string message, Exception innerException) : base(message, innerException)
    {

    }
}