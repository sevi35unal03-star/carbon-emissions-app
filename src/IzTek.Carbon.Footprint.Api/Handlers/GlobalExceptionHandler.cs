namespace IzTek.Carbon.Footprint.Api.Handlers;

/// <summary>
/// Global exception handler for centralized error handling and response formatting
/// Implements IExceptionHandler for .NET 8+ minimal APIs
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment) : IExceptionHandler
{
    private const string DefaultErrorMessage = "An error occurred while processing your request";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        LogException(httpContext, exception);

        var statusCode = GetStatusCode(exception);
        var errors = GetErrors(exception);

        var response = Result.Failure(errors, statusCode);

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json; charset=utf-8";

        await httpContext.Response.WriteAsJsonAsync(
            response,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            },
            cancellationToken);

        return true;
    }

    private static HttpStatusCode GetStatusCode(Exception exception) =>
        exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            BadRequestException => HttpStatusCode.BadRequest,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            Application.Common.Exceptions.ValidationException => HttpStatusCode.UnprocessableEntity,
            _ => HttpStatusCode.InternalServerError
        };

    private static ErrorCode GetErrorCode(Exception exception) =>
        exception switch
        {
            NotFoundException => SystemErrorCodes.NotFound,
            BadRequestException => SystemErrorCodes.BadRequest,
            ConflictException => SystemErrorCodes.ConflictError,
            UnauthorizedException => SystemErrorCodes.Unauthorized,
            ForbiddenException => SystemErrorCodes.AccessDenied,
            Application.Common.Exceptions.ValidationException => SystemErrorCodes.ValidationError,
            _ => SystemErrorCodes.SystemError
        };

    private List<ErrorResult> GetErrors(Exception exception)
    {
        var errorCode = GetErrorCode(exception);

        if (exception is Application.Common.Exceptions.ValidationException validationException)
        {
            return CreateValidationErrors(validationException, errorCode);
        }

        return CreateSingleError(exception, errorCode);
    }

    private static List<ErrorResult> CreateValidationErrors(
        Application.Common.Exceptions.ValidationException validationException,
        ErrorCode errorCode)
    {
        var errors = new List<ErrorResult>();

        foreach (var (field, errorMessages) in validationException.Errors)
        {
            foreach (var errorMessage in errorMessages)
            {
                var error = new ErrorResult(
                    errorCode: errorCode,
                    message: $"{field}: {errorMessage}",
                    isShow: true);

                errors.Add(error);
            }
        }

        return errors.Count > 0
            ? errors
            : [new ErrorResult(errorCode, "Validation failed", isShow: true)];
    }

    private List<ErrorResult> CreateSingleError(Exception exception, ErrorCode errorCode)
    {
        if (exception is not ApplicationException)
        {
            var message = environment.IsDevelopment()
                ? exception.Message
                : DefaultErrorMessage;

            var error = new ErrorResult(
                errorCode: errorCode,
                message: message,
                isShow: environment.IsDevelopment());

            return [error];
        }

        var appError = new ErrorResult(
            errorCode: errorCode,
            message: exception.Message,
            isShow: true);

        return [appError];
    }

    private void LogException(HttpContext httpContext, Exception exception)
    {
        var request = httpContext.Request;

        var logMessage = new
        {
            Exception = exception.GetType().Name,
            Message = exception.Message,
            StackTrace = exception.StackTrace,
            InnerException = exception.InnerException?.Message,
            Request = new
            {
                Path = request.Path.ToString(),
                Method = request.Method,
                QueryString = request.QueryString.ToString(),
                Headers = GetSafeHeaders(request.Headers),
                ContentType = request.ContentType
            },
            User = new
            {
                IsAuthenticated = httpContext.User?.Identity?.IsAuthenticated ?? false,
                UserId = httpContext.User?.FindFirst("sub")?.Value ??
                        httpContext.User?.FindFirst("userId")?.Value,
                UserName = httpContext.User?.Identity?.Name
            },
            Connection = new
            {
                RemoteIp = httpContext.Connection.RemoteIpAddress?.ToString(),
                LocalIp = httpContext.Connection.LocalIpAddress?.ToString()
            },
            TraceId = httpContext.TraceIdentifier,
            Timestamp = DateTimeOffset.UtcNow
        };

        if (exception is ApplicationException)
        {
            logger.LogWarning(
                exception,
                "Application exception occurred. {@Details}",
                logMessage);
        }
        else
        {
            logger.LogError(
                exception,
                "Unhandled exception occurred. {@Details}",
                logMessage);
        }
    }

    private static Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var sensitiveHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Authorization",
            "Cookie",
            "X-API-Key",
            "X-Auth-Token"
        };

        return headers
            .Where(h => !sensitiveHeaders.Contains(h.Key))
            .ToDictionary(h => h.Key, h => h.Value.ToString());
    }
}