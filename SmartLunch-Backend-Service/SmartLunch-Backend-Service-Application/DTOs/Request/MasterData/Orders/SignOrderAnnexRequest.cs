namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;

/// <summary>Chữ ký PNG (data URL) để ghép vào PDF phụ lục đặt hàng (mô phỏng ký số).</summary>
public class SignOrderAnnexRequest
{
    public string DigitalSignature { get; set; } = string.Empty;
}
