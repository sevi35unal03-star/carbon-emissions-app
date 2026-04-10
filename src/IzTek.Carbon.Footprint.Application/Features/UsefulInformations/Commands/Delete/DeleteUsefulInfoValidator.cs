namespace IzTek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Delete
{
    public class DeleteUsefulInfoValidator : AbstractValidator<DeleteUsefulInformationsCommand>
    {
        public DeleteUsefulInfoValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required for deletion.");
        }
    }
}
