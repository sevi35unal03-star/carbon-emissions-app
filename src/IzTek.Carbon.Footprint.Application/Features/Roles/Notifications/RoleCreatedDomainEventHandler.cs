namespace IzTek.Carbon.Footprint.Application.Features.Roles.Notifications;

public static class RoleCreatedDomainEventHandler
{
    public static async Task HandleAsync(
        RoleCreatedDomainEvent @event,
        IPlatformService platformService,
        ILogger<RoleCreatedDomainEvent> logger,
        CancellationToken cancellationToken)
    {
        // 1. Start of Processing Log
        logger.LogInformation("Processing role creation notification. Role ID: {RoleId}, Name: {RoleName}, Type: {RoleType}",
            @event.Id, @event.Name, @event.Type);

        try
        {
            // 2. Business Logic: Prepare notification content
            var message = $"A new role has been defined in the system: {@event.Name} (Type: {@event.Type})";

            // 3. Action: Send Email
            await platformService.SendEmailAsync("admin@iztek.com", message);

            // 4. Success Log
            logger.LogInformation("Role creation notification sent successfully for Role ID: {RoleId}", @event.Id);
        }
        catch (Exception ex)
        {
            // 5. Error Log
            logger.LogError(ex, "An error occurred while sending role creation notification. Role ID: {RoleId}", @event.Id);
        }
    }
}