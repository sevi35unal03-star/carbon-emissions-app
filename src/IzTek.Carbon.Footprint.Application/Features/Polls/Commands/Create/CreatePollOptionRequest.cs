namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public class CreatePollOptionRequest
{
    public string Text { get; set; }    
    public double Value { get; set; }       
    public int DisplayOrder { get; set; }  
}