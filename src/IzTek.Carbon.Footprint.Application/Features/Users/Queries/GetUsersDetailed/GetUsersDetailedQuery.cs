namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;

public record GetUsersDetailedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    bool? ShowDeleted = null);