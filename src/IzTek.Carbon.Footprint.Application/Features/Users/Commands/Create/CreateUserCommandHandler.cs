using IzTek.Carbon.Footprint.Domain.Events.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Create;

public class CreateUserCommandHandler(
    UserManager<User> userManager,
    ILogger<CreateUserCommandHandler> logger)
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            PollQuestionId = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.IdentityNumber, // Giriş anahtarı olarak T.C. No kullanıyoruz
            Name = request.FirstName,
            Surname = request.LastName,
            IdentityNumber = request.IdentityNumber,
            PhoneNumber = request.PhoneNumber,
            BirthDate = request.BirthDate,
            EmailConfirmed = true, // Mobil senaryoda genellikle varsayılan true tutulur veya OTP istenir
            IsKvkkApproved = request.IsKvkkApproved,
            KvkkApprovalDate = DateTime.UtcNow,
        };

        // 2. Identity üzerinden kullanıcıyı oluştur (Şifre burada otomatik hashlenir)
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessage = result.Errors.First().Description;
            logger.LogError("User creation failed: {Error}", errorMessage);
            //return Result<Guid>.Failure(errorMessage);
        }

        // 3. Kullanıcıya varsayılan rolü ata (Örn: "User")
        await userManager.AddToRoleAsync(user, "User");

        // 4. Domain Event fırlat (Hoş geldin maili veya loglama için)
        user.AddDomainEvent(new UserRegisteredDomainEvent(user));

        logger.LogInformation("User created successfully with Identity Number: {IdentityNumber}", user.IdentityNumber);

        return Result<Guid>.Success(user.PollQuestionId);
    }
}