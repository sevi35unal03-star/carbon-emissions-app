using System.Net;
using System.Text.Json.Serialization;

namespace IzTek.Carbon.Footprint.Application.Common.Models;

public class Result
{
    [JsonPropertyName("isSuccessful")]
    public bool IsSuccessful { get; init; }

    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; init; }

    [JsonPropertyName("errors")]
    public List<ErrorResult> Errors { get; init; }

    public static Result Created() => new() { StatusCode = HttpStatusCode.Created, IsSuccessful = true };

    public static Result NoContent() => new() { StatusCode = HttpStatusCode.NoContent, IsSuccessful = true };

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK) => new() { StatusCode = statusCode, IsSuccessful = true };

    public static Result Failure(ErrorResult errors, HttpStatusCode statusCode) => new() { Errors = [errors], StatusCode = statusCode, IsSuccessful = false };

    public static Result Failure(List<ErrorResult> errors, HttpStatusCode statusCode) => new() { Errors = errors, StatusCode = statusCode, IsSuccessful = false };

    public static Result Failure(ErrorCode errorCode, HttpStatusCode statusCode, bool isShow = true)
    {
        var error = new ErrorResult(errorCode, isShow: isShow);
        return new Result { Errors = [error], StatusCode = statusCode, IsSuccessful = false };
    }

    public static Result Failure(ErrorCode errorCode, string message, HttpStatusCode statusCode, bool isShow = true)
    {
        var error = new ErrorResult(errorCode, message, isShow);
        return new Result { Errors = [error], StatusCode = statusCode, IsSuccessful = false };
    }

    public static Result SystemException()
    {
        var error = new ErrorResult(SystemErrorCodes.SystemError, false);
        return new Result { Errors = [error], StatusCode = HttpStatusCode.InternalServerError, IsSuccessful = false };
    }

    //internal static Result Failure(string v, HttpStatusCode notFound)
    //{
        //throw new NotImplementedException();
    //}
}

public class Result<T> : Result
{
    [JsonPropertyName("data")]
    public T Data { get; init; }

    public static Result<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK) => new() { Data = data, StatusCode = statusCode, IsSuccessful = true };

    public new static Result<T> Failure(ErrorResult errors, HttpStatusCode statusCode) => new() { Errors = [errors], StatusCode = statusCode, IsSuccessful = false };

    public new static Result<T> Failure(List<ErrorResult> errors, HttpStatusCode statusCode) => new() { Errors = errors, StatusCode = statusCode, IsSuccessful = false };

    public new static Result<T> Failure(ErrorCode errorCode, HttpStatusCode statusCode, bool isShow = true)
    {
        var error = new ErrorResult(errorCode, isShow: isShow);
        return new Result<T> { Errors = [error], StatusCode = statusCode, IsSuccessful = false };
    }

    public new static Result<T> Failure(ErrorCode errorCode, string message, HttpStatusCode statusCode, bool isShow = true)
    {
        var error = new ErrorResult(errorCode, message, isShow);
        return new Result<T> { Errors = [error], StatusCode = statusCode, IsSuccessful = false };
    }

    public static Result<T> Failure(ErrorCode errorCode, string[] messageArg, HttpStatusCode statusCode, bool isShow = true)
    {
        var error = new ErrorResult(errorCode, messageArg, isShow);
        return new Result<T> { Errors = [error], StatusCode = statusCode, IsSuccessful = false };
    }

    public new static Result<T> SystemException()
    {
        var error = new ErrorResult(SystemErrorCodes.SystemError, false);
        return new Result<T> { Errors = [error], StatusCode = HttpStatusCode.InternalServerError, IsSuccessful = false };
    }
}