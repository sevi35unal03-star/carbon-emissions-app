using System.Text.Json.Serialization;

namespace IzTek.Carbon.Footprint.Application.Common.Models;

public class ErrorResult
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("service")] public string Service { get; set; }
    [JsonPropertyName("isShow")] public bool IsShow { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; }
    [JsonIgnore] public string[] MessageArgs { get; set; }

    public ErrorResult()
    {
    }

    public ErrorResult(ErrorCode errorCode, bool isShow = true)
    {
        IsShow = isShow;
        Code = errorCode.Code;
        Type = errorCode.Name;
        Service = errorCode.Service;
        MessageArgs = null;
    }

    public ErrorResult(ErrorCode errorCode, string message, bool isShow = true)
    {
        IsShow = isShow;
        Code = errorCode.Code;
        Type = errorCode.Name;
        Service = errorCode.Service;
        Message = message;
    }

    public ErrorResult(ErrorCode errorCode, string[] messageArgs = null, bool isShow = true)
    {
        IsShow = isShow;
        Code = errorCode.Code;
        Type = errorCode.Name;
        Service = errorCode.Service;
        MessageArgs = messageArgs;
    }
}