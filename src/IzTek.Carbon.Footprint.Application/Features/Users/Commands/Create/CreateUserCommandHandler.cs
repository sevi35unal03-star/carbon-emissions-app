using IzTek.Carbon.Footprint.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.Create;

public class CreateUserCommandHandler(
    UserManager<User> userManager,
    ILogger<CreateUserCommandHandler> logger)
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(
            email: request.Email,
            name: request.FirstName,
            surname: request.LastName,
            birthDate: DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
            identityNumber: request.IdentityNumber,
            phoneNumber: request.PhoneNumber,
            isKvkkApproved: request.IsKvkkApproved
        );

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessage = result.Errors.First().Description;
            logger.LogError("User creation failed: {Error}", errorMessage);

            // Identity hata mesajına göre doğru kodu seç
            var errorCode = errorMessage.Contains("Email") ? SystemErrorCodes.EmailAlreadyExists
                : errorMessage.Contains("UserName") ? SystemErrorCodes.IdentityNumberAlreadyExists
                : errorMessage.Contains("phone") ? SystemErrorCodes.PhoneNumberAlreadyExists
                : SystemErrorCodes.BadRequest;

            return Result<Guid>.Failure(SystemErrorCodes.BadRequest, errorMessage, HttpStatusCode.BadRequest);
        }

        await userManager.AddToRoleAsync(user, "User");

        logger.LogInformation("User created successfully with Identity Number: {IdentityNumber}", user.IdentityNumber);

        return Result<Guid>.Success(user.Id);
    }
}