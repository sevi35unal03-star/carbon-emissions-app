namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class NotFoundException(string message) : ApplicationException(message)
{
}