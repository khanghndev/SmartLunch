using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Ingredients;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class IngredientRequestValidation
{
    public static void ValidateCreate(CreateIngredientRequest req)
    {
        ValidateCore(req.Name, req.Unit, req.CostPerUnit, req.NameEnglish, req.Description);
    }

    public static void ValidateUpdate(UpdateIngredientRequest req)
    {
        ValidateCore(req.Name, req.Unit, req.CostPerUnit, req.NameEnglish, req.Description);
    }

    private static void ValidateCore(
        string name,
        string unit,
        decimal? costPerUnit,
        string? nameEnglish,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");
        if (name.Trim().Length > 255)
            throw new ArgumentException("Name must not exceed 255 characters.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit is required.");
        if (unit.Trim().Length > 20)
            throw new ArgumentException("Unit must not exceed 20 characters.");

        if (!string.IsNullOrWhiteSpace(nameEnglish) && nameEnglish.Trim().Length > 100)
            throw new ArgumentException("NameEnglish must not exceed 100 characters.");

        if (!string.IsNullOrWhiteSpace(description) && description.Trim().Length > 255)
            throw new ArgumentException("Description must not exceed 255 characters.");

        if (costPerUnit.HasValue && costPerUnit.Value < 0)
            throw new ArgumentException("CostPerUnit cannot be negative.");
    }
}
