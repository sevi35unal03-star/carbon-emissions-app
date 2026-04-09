using FluentValidation;
using ValidationException = IzTek.Carbon.Footprint.Application.Common.Exceptions.ValidationException;

namespace IzTek.Carbon.Footprint.Application.Common.Behaviors;

/// <summary>
/// Command ve Query’ler işlenmeden önce (handler’a girmeden önce) otomatik olarak
/// validation kurallarını çalıştırmak ve kurallara uymayan istekleri erken bir şekilde engellemektir.
/// </summary>
public class ValidationBehavior
{
    private readonly IEnumerable<IValidator> _validators;

    public ValidationBehavior(IEnumerable<IValidator> validators)
    {
        _validators = validators;
    }

    public async Task BeforeAsync(IMessage message, CancellationToken ct)
    {
        // Mesaj tipi için geçerli validator'ları filtrele
        var messageType = message.GetType();

        var applicableValidators = _validators
            .Where(v => v.CanValidateInstancesOfType(messageType))
            .ToList();

        if (applicableValidators.Count == 0)
            return;

        var context = new ValidationContext<object>(message);

        var validationResults = await Task.WhenAll(
            applicableValidators.Select(v => v.ValidateAsync(context, ct)));

        var errors = new Dictionary<string, List<string>>();

        foreach (var result in validationResults)
        {
            foreach (var error in result.Errors)
            {
                if (error == null) continue;

                if (!errors.ContainsKey(error.PropertyName))
                    errors[error.PropertyName] = new List<string>();

                errors[error.PropertyName].Add(error.ErrorMessage);
            }
        }

        if (errors.Any())
        {
            throw new ValidationException(errors);
        }
    }
}