using FluentValidation;
using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Common.Models;
using IzTek.Carbon.Footprint.Domain.Entities;
using System.Net;

namespace Iztek.Carbon.Footprint.Application.Features.UsefulInformations.Commands.Create;

public class CreateUsefulInformationsCommand
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public int DisplayOrder { get; set; }
}

public class CreateUsefulInformationsCommandValidator : AbstractValidator<CreateUsefulInformationsCommand>
{
    public CreateUsefulInformationsCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
    }
}


