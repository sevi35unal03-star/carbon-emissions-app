namespace IzTek.Carbon.Footprint.Infrastructure.Validators;

public class PasswordValidator : IPasswordValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password)
    {
        var errors = new List<IdentityError>();

        if (string.IsNullOrWhiteSpace(password))
            return Task.FromResult(IdentityResult.Failed());

        if (password.Contains(user.UserName!, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordContainsUserName",
                Description = "Password cannot contain your username."
            });
        }

        var emailPart = user.Email?.Split('@')[0];
        if (!string.IsNullOrEmpty(emailPart) && password.Contains(emailPart, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordContainsEmail",
                Description = "Password cannot contain parts of your email address."
            });
        }

        return Task.FromResult(errors.Count == 0
            ? IdentityResult.Succeeded
            : IdentityResult.Failed(errors.ToArray()));
    }
}