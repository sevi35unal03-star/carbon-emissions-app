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

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(e => e != null)
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToList());

        if (errors.Any())
        {
            throw new ValidationException(errors);
        }
    }
}