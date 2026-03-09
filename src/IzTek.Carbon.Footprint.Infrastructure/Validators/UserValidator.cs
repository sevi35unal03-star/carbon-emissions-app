namespace IzTek.Carbon.Footprint.Infrastructure.Validators;

public class UserValidator : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        var errors = new List<IdentityError>();

        if (string.IsNullOrWhiteSpace(user.IdentityNumber) || user.IdentityNumber.Length != 11 || !long.TryParse(user.IdentityNumber, out _))
        {
            errors.Add(new IdentityError
            {
                Code = "InvalidIdentityNumber",
                Description = "Identity number must be exactly 11 digits."
            });
        }

        if (!user.IsKvkkApproved)
        {
            errors.Add(new IdentityError
            {
                Code = "KvkkNotApproved",
                Description = "KVKK approval is mandatory."
            });
        }

        return Task.FromResult(errors.Count == 0
            ? IdentityResult.Succeeded
            : IdentityResult.Failed(errors.ToArray()));
    }
}