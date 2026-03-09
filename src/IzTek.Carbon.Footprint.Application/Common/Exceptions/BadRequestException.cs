namespace IzTek.Carbon.Footprint.Application.Common.Exceptions;

public class BadRequestException(string message) : ApplicationException(message)
{
}