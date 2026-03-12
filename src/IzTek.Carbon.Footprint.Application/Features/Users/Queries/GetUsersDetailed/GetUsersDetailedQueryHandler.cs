using Microsoft.AspNetCore.Identity;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUsersDetailed;

public class GetUsersDetailedQueryHandler(UserManager<User> userManager)
{
    public async Task<PagedResult<List<GetUsersDetailedResponse>>> Handle(
        GetUsersDetailedQuery request, 
        CancellationToken ct)
    {
        // 1. IQueryable sorgusunu başlat
        var query = userManager.Users.AsNoTracking();

        // 2. Arama Filtresi (İsim, Soyisim veya TCKN)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(searchTerm) ||
                x.Surname.ToLower().Contains(searchTerm) ||
                x.IdentityNumber.Contains(searchTerm));
        }

        // 3. Silinmiş Kullanıcı Filtresi
        if (request.ShowDeleted.HasValue)
        {
            query = query.Where(x => x.IsDeleted == request.ShowDeleted.Value);
        }

        // 4. Toplam Kayıt Sayısı (Hesaplamalar için şart)
        var totalCount = await query.CountAsync(ct);

        // 5. Sayfalama ve Projeksiyon
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

        // 6. Senin modelin olan PagedResult ile sarmala
        return PagedResult<List<GetUsersDetailedResponse>>.Success(
            data: users,
            totalCount: totalCount,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize
        );
    }
}