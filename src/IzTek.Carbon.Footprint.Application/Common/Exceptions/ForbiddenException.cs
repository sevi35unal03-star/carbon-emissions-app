namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class ForbiddenException(string message) : ApplicationException(message)
{
}