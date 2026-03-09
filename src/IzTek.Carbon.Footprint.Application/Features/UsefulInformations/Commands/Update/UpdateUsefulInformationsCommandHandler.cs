namespace Iztek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Update;

public static class UpdateUsefulInformationCommandHandler
{
    public static async Task<Result> HandleAsync(
        UpdateUsefulInformationsCommand command,
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var info = await context.UsefulInformations
            .FirstOrDefaultAsync(x => x.PollQuestionId == command.Id, cancellationToken);

        if (info is null)
        {
            return Result.Failure("InformationNotFound", HttpStatusCode.NotFound);
        }

        
        var isTitleExists = await context.UsefulInformations
            .AnyAsync(x => x.Title == command.Title && x.PollQuestionId != command.Id, cancellationToken);

        if (isTitleExists)
        {
            return Result.Failure("InformationTitleAlreadyExists", HttpStatusCode.BadRequest);
        }

        
        info.Update(command.Title, command.Content, command.DisplayOrder);

        
        var success = await context.SaveChangesAsync(cancellationToken) > 0;

        return success
            ? Result.Success()
            : Result.Failure("UpdateFailed", HttpStatusCode.InternalServerError);
    }
}