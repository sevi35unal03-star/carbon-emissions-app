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

    public async Task<Result> Handle(
        DeleteUsefulInformationsCommand command, 
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var info = await context.UsefulInformations
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (info == null) return Result.Failure(SystemErrorCodes.NotFound, HttpStatusCode.NotFound);

        info.IsDeleted = true;
        info.DeletedAt = DateTime.UtcNow;

        context.UsefulInformations.Remove(info);
        return await context.SaveChangesAsync(ct) > 0
    ? Result.NoContent()
    : Result.Failure(SystemErrorCodes.DeleteFailed, HttpStatusCode.InternalServerError);
    }
}

