using System.Reflection;

namespace IzTek.Carbon.Footprint.Application.Common.Models;

public static class SystemErrorCodes
{
    private const string SERVICE_NAME = "System";

    public static readonly ErrorCode SystemError = new SystemErrorCode(1001, nameof(SystemError));
    public static readonly ErrorCode NotFound = new SystemErrorCode(1002, nameof(NotFound));
    public static readonly ErrorCode BadRequest = new SystemErrorCode(1003, nameof(BadRequest));
    public static readonly ErrorCode ValidationError = new SystemErrorCode(1004, nameof(ValidationError));
    public static readonly ErrorCode ConflictError = new SystemErrorCode(1005, nameof(ConflictError));
    public static readonly ErrorCode QuestionCreationFailed = new SystemErrorCode(1006, nameof(QuestionCreationFailed));

    public static readonly ErrorCode Unauthorized = new SystemErrorCode(1101, nameof(Unauthorized));
    public static readonly ErrorCode AccessDenied = new SystemErrorCode(1102, nameof(AccessDenied));
    public static readonly ErrorCode SessionExpired = new SystemErrorCode(1103, nameof(SessionExpired));

    public static readonly ErrorCode TooManyRequests = new SystemErrorCode(1201, nameof(TooManyRequests));

    public static readonly ErrorCode InvalidParameter = new SystemErrorCode(1301, nameof(InvalidParameter));

    public static readonly ErrorCode ProductAlreadyExists = new SystemErrorCode(1302, nameof(ProductAlreadyExists));

    public static readonly ErrorCode RoleAlreadyExists = new SystemErrorCode(1303, nameof(RoleAlreadyExists));

    

    private sealed class SystemErrorCode(int code, string name) : ServiceErrorCode(code, name, SERVICE_NAME)
    {
    }

    public static IEnumerable<ErrorCode> GetAll()
    {
        return typeof(SystemErrorCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(ErrorCode))
            .Select(f => (ErrorCode)f.GetValue(null)!)
            .Where(code => code != null);
    }
}