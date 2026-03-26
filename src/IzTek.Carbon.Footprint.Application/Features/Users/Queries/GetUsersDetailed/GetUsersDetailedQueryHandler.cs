using Microsoft.AspNetCore.Identity;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;

public static class GetUsersDetailedQueryHandler
{
    public static async Task<PagedResult<List<GetUsersDetailedResponse>>> Handle(
        GetUsersDetailedQuery request,
        UserManager<User> userManager,
        CancellationToken ct)
    {
        var query = userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(searchTerm) ||
                x.Surname.ToLower().Contains(searchTerm) ||
                x.IdentityNumber.Contains(searchTerm));
        }

        if (request.ShowDeleted.HasValue)
            query = query.Where(x => x.IsDeleted == request.ShowDeleted.Value);

        var totalCount = await query.CountAsync(ct);

        var users = await query
            .OrderByDescending(x => x.TotalPoints)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(user => new GetUsersDetailedResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                IdentityNumber = user.IdentityNumber,
                TotalCarbonScore = user.TotalPoints,
                IsKvkkApproved = user.IsKvkkApproved,
                BirthDate = user.BirthDate,
                IsDeleted = user.IsDeleted,
                DeletedDate = user.DeletedDate
            })
            .ToListAsync(ct);

        return PagedResult<List<GetUsersDetailedResponse>>.Success(
            data: users,
            totalCount: totalCount,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize);
    }
}