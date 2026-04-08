//Admin paneli için UserPollResult tablosundaki en güncel verileri listeler
namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserDailyResults;


public record GetUserDailyResultsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    bool? ShowDeleted = null);