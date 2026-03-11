

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public class CreatePollQuestionRequest
{
    public string Text { get; set; } = default!;
    public int DisplayOrder { get; set; }

    public List<CreatePollOptionRequest> Options { get; set; } = [];
};
