namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Ingredients;

public class DeleteIngredientResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    /// <summary>True nếu chỉ đặt IsActive=false (còn được dùng trong món / phiếu nhập).</summary>
    public bool DeactivatedOnly { get; set; }
}
