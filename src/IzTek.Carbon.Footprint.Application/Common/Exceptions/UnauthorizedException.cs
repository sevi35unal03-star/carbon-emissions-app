namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class UnauthorizedException(string message) : ApplicationException(message)
{
}