namespace SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;

public class ReviewIngredientIntakeProposalRequest
{
    public bool Approve { get; set; }
    public string? ReviewNote { get; set; }
}
