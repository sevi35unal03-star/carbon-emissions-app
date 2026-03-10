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
        var user = new User(
            emailConfirmed: true,
            email: request.Email,
            name: request.FirstName,
            surname: request.LastName,
            birthDate: request.BirthDate,
            identityNumber: request.IdentityNumber,
            phoneNumber: request.PhoneNumber,
            password: request.Password,
            confirmPassword: request.ConfirmPassword,
            isKvkkApproved: request.IsKvkkApproved,
            kvkkApprovalDate: DateTime.UtcNow,
            lastCarbonScore: 0,
            totalPoints: 0,
            isDeleted: false,
            deletedDate: null,
            totalCarbonScore: 0,
            lastLoginDate: DateTime.UtcNow,
            totalCarbonPoint: 0,
            donatedTreeCount: 0,
            lastDonationDate: DateTime.UtcNow
);

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

        logger.LogInformation("User created successfully with Identity Number: {IdentityNumber}", user.IdentityNumber);

        return Result<Guid>.Success(user.PollQuestionId);
    }
}