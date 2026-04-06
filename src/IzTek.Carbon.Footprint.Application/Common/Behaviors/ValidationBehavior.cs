using ValidationException = IzTek.Carbon.Footprint.Application.Common.Exceptions.ValidationException;
namespace IzTek.Carbon.Footprint.Application.Common.Behaviors
{
    public class ValidationBehavior
    { 

        public static async Task BeforeAsync(IEnumerable<IValidator> validators,
            IMessage message,
            CancellationToken ct)
        {

            if (!validators.Any()) return;
            var validationcontext = new ValidationContext<object>(message);
            var errors=validators
            .Select(x => x.Validate(validationcontext))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToList()
            );

            if (errors.Any())
                throw new ValidationException(errors);

        }


    }
}
