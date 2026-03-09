namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Delete;

public class DeleteUsefulInfoValidator : AbstractValidator<DeleteUsefulInformationsCommand>
{
    public DeleteUsefulInfoValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required for deletion.");
    }
}

public class DeleteUsefulInformationsCommandHandler
{
    private readonly bool success;

    public async Task<Result> Handle(
        DeleteUsefulInformationsCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var info = await context.UsefulInformations
            .FirstOrDefaultAsync(x => x.PollQuestionId == command.Id, ct);

        if (info == null) return Result.Failure("Information not found.", HttpStatusCode.NotFound);

        info.IsDeleted = true;
        info.DeletedAt = DateTime.UtcNow;

        context.UsefulInformations.Remove(info);
        await context.SaveChangesAsync(ct);

        return success
            ? Result.Success()
            : Result.Failure("DeleteFailed", HttpStatusCode.InternalServerError);
    }
}

