namespace SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;

public class CreateActualIntakeFromProposalRequest
{
    /// <summary>Bắt buộc true: xác nhận nguyên liệu nhập đạt chuẩn theo phiếu đã duyệt.</summary>
    public bool? ConfirmIngredientsMeetStandard { get; set; }

    public DateTime? ReceivedAtUtc { get; set; }
    public string? Note { get; set; }
}
