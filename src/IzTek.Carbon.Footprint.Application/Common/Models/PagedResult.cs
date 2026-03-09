using System.Net;
using System.Text.Json.Serialization;

namespace IzTek.Carbon.Footprint.Application.Common.Models;

public sealed class PagedResult<T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; }

    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => TotalCount / PageSize + (TotalCount % PageSize > 0 ? 1 : 0);

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage => PageNumber + 1 <= TotalPages;

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage => PageNumber - 1 > 0;

    [JsonPropertyName("statusCode")]
    public HttpStatusCode StatusCode { get; set; }

    [JsonPropertyName("isSuccessful")]
    public bool IsSuccessful { get; set; }

    [JsonPropertyName("errors")]
    public List<ErrorResult> Errors { get; set; }

    public static PagedResult<T> Success(T data, int totalCount, int pageNumber, int pageSize, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new PagedResult<T>
        {
            Data = data,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            StatusCode = statusCode,
            IsSuccessful = true
        };
    }
}