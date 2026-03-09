namespace IzTek.Carbon.Footprint.Api.Controllers;

public abstract class BaseController(IStringLocalizer localizer) : ControllerBase
{
    [NonAction]
    public IActionResult CreateActionResultInstance<T>(T response) where T : Result
    {
        if (!response.IsSuccessful)
        {
            SetLanguageMessage(response.Errors);
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return NoContent();
        }

        return new ObjectResult(response)
        {
            StatusCode = (int)response.StatusCode,
        };
    }

    [NonAction]
    public IActionResult CreateActionResultInstance<T>(PagedResult<T> response)
    {
        if (!response.IsSuccessful)
        {
            SetLanguageMessage(response.Errors);
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return NoContent();
        }

        return new ObjectResult(response)
        {
            StatusCode = (int)response.StatusCode,
        };
    }

    private void SetLanguageMessage(List<ErrorResult> errors)
    {
        foreach (var error in errors)
        {
            if (error.MessageArgs?.Length > 0)
            {
                error.Message = localizer.GetString(error.Type, error.MessageArgs);
            }
            else if (string.IsNullOrWhiteSpace(error.Message))
            {
                error.Message = localizer.GetString(error.Type);
            }
        }
    }
}