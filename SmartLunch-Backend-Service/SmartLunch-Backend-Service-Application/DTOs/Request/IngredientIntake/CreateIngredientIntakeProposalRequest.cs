namespace SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;

public class CreateIngredientIntakeProposalRequest
{
    public string? HeaderNote { get; set; }
    public List<CreateIngredientIntakeProposalLineRequest> Lines { get; set; } = new();
}

public class CreateIngredientIntakeProposalLineRequest
{
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string? LineNote { get; set; }
}
