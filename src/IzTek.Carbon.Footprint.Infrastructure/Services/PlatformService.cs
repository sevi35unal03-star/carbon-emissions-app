namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class PlatformService(IHttpClientFactory httpClientFactory) : IPlatformService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("platform");

    public async Task<Result?> SendEmailAsync(string to, string subject, string content)
    {
        var responseMessage = await _httpClient.PostAsJsonAsync("/iztek/mail", new { to, subject, content });

        if (responseMessage.IsSuccessStatusCode)
            return Result.Success();

        return await responseMessage.DeserializeAsync<Result>();
    }

    public async Task<Result?> SendSmsAsync(string phoneNumber, string message)
    {
        var responseMessage = await _httpClient.PostAsJsonAsync("/iztek/sms", new
        {
            to = phoneNumber,
            message
        });

        if (responseMessage.IsSuccessStatusCode)
            return Result.Success();

        return await responseMessage.DeserializeAsync<Result>();
    }

    public async Task<Result?> SendPushNotificationAsync(string target, string title, string body)
    {
        var responseMessage = await _httpClient.PostAsJsonAsync("/iztek/push", new
        {
            target,
            title,
            body
        });

        if (responseMessage.IsSuccessStatusCode)
            return Result.Success();

        return await responseMessage.DeserializeAsync<Result>();
    }

    // ✅ Eklendi
    public async Task<Result?> SendPushToAllUsersAsync(string title, string body, object? data = null)
    {
        var responseMessage = await _httpClient.PostAsJsonAsync("/iztek/push/all", new
        {
            target = "AllUsers",
            title,
            body,
            data
        });

        if (responseMessage.IsSuccessStatusCode)
            return Result.Success();

        return await responseMessage.DeserializeAsync<Result>();
    }

    // ✅ Eklendi
    public async Task<Result?> SendPushToUserAsync(string userId, string title, string body, object? data = null)
    {
        var responseMessage = await _httpClient.PostAsJsonAsync("/iztek/push/user", new
        {
            target = userId,
            title,
            body,
            data
        });

        if (responseMessage.IsSuccessStatusCode)
            return Result.Success();

        return await responseMessage.DeserializeAsync<Result>();
    }

    public async Task<Result?> RegisterUserAsync(string email, string name, string surname, string phoneNumber)
    {
        var responseMessage = await _httpClient.PostAsJsonAsync("/iztek/register", new
        {
            email,
            name,
            surname,
            phoneNumber
        });

        if (responseMessage.IsSuccessStatusCode)
            return Result.Success();

        return await responseMessage.DeserializeAsync<Result>();
    }

    public async Task<Result?> ValidateUserAsync(string bizIzmirToken)
    {
        // TODO: BizIzmir endpoint'i netleşince güncellenecek
        throw new NotImplementedException("BizIzmir entegrasyonu bekleniyor.");
    }
}

/*
    PlatformService, uygulamanın diğer bölümlerinin e-posta gönderme ve push bildirimleri gibi platforma özgü işlemleri gerçekleştirmesine olanak tanır.
    Bu servis, HTTP istemcisi aracılığıyla platformun API'sine istekler göndererek bu işlemleri gerçekleştirir.
*/

// Ileride buraya RegisterAsync eklenecek -> BizIzmir servisi