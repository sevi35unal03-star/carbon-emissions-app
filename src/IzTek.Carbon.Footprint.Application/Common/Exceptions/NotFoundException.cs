namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException() : base("The requested resource was not found.")
    {

    }

    public NotFoundException(string message) : base(message)
    {

    }

    public NotFoundException(string name, object key) : base($"'{name}' with key '{key}' was not found.")
    {

    }
}