namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class ConflictException : ApplicationException
{
    public ConflictException() : base("A conflict occurred with the current state of the resource.")
    {

    }

    public ConflictException(string message) : base(message)
    {

    }

    public ConflictException(string resource, string reason) : base($"Conflict on '{resource}': {reason}")
    {

    }
}