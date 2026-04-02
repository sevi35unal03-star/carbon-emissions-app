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
        // 1. Önce unique kontroller yap — DB constraint hatasından önce yakala
        var identityExists = await userManager.Users
            .AnyAsync(x => x.IdentityNumber == request.IdentityNumber, cancellationToken);

        if (identityExists)
            return Result<Guid>.Failure(
                SystemErrorCodes.IdentityNumberAlreadyExists, HttpStatusCode.Conflict);

        var emailExists = await userManager.FindByEmailAsync(request.Email);
        if (emailExists is not null)
            return Result<Guid>.Failure(
                SystemErrorCodes.EmailAlreadyExists, HttpStatusCode.Conflict);

        var phoneExists = await userManager.Users
            .AnyAsync(x => x.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (phoneExists)
            return Result<Guid>.Failure(
                SystemErrorCodes.PhoneNumberAlreadyExists, HttpStatusCode.Conflict);

        // 2. Kullanıcı oluştur
        var user = new User(
            email: request.Email,
            name: request.FirstName,
            surname: request.LastName,
            birthDate: DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
            identityNumber: request.IdentityNumber,
            phoneNumber: request.PhoneNumber,
            isKvkkApproved: request.IsKvkkApproved);

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessage = result.Errors.First().Description;
            logger.LogError("User creation failed: {Error}", errorMessage);
            return Result<Guid>.Failure(SystemErrorCodes.BadRequest, errorMessage, HttpStatusCode.BadRequest);
        }

        await userManager.AddToRoleAsync(user, "User");
        logger.LogInformation("User created successfully → UserId: {UserId}", user.Id);

        return Result<Guid>.Success(user.Id);
    }
}