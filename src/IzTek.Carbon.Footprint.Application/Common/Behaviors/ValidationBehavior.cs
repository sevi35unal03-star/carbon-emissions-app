using ValidationException = IzTek.Carbon.Footprint.Application.Common.Exceptions.ValidationException;
namespace IzTek.Carbon.Footprint.Application.Common.Behaviors
{
    public class ValidationBehavior
    {
        private readonly IEnumerable<IValidator> _validators;

        // Wolverine constructor injection ile resolve eder
        public ValidationBehavior(IEnumerable<IValidator> validators)
            => _validators = validators;

        public async Task BeforeAsync(
            IMessage message,        // ← Wolverine mesajı inject eder
            CancellationToken ct)
        {
            if (!_validators.Any()) return;

            var validationContext = new ValidationContext<object>(message);

            var errors = (await Task.WhenAll(
                    _validators.Select(x => x.ValidateAsync(validationContext, ct))))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToList());

            if (errors.Any())
                throw new ValidationException(errors);
        }

    }

}
