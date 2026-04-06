namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException() : base("You are not authorized to perform this action.")
    {

    }

    public UnauthorizedException(string message) : base(message)
    {

    }

    public UnauthorizedException(string resource, string action) : base($"You are not authorized to perform '{action}' on '{resource}'.")
    {

    }
}