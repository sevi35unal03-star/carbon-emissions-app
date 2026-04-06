namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class ForbiddenException : ApplicationException
{
    public ForbiddenException() : base("You do not have permission to access this resource.")
    {

    }

    public ForbiddenException(string message) : base(message)
    {

    }

    public ForbiddenException(string resource, string role) : base($"Access to '{resource}' requires '{role}' role.")
    {

    }
}