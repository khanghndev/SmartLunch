using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Infrastructure.Data;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// Dish images management (cover + gallery).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/Dish/{dishId:int}/images")]
[Authorize(Roles = "Admin,Manager")]
public class DishImagesController : ControllerBase
{
    private readonly ILogger<DishImagesController> _logger;
    private readonly SmartLunchDBContext _db;

    public DishImagesController(ILogger<DishImagesController> logger, SmartLunchDBContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost]
    [Authorize(Policy = "permission:dishes.update")]
    public async Task<ActionResult<BaseApiResponse<object>>> AddImage(int dishId, [FromBody] AddDishImageRequest request)
    {
        try
        {
            var dishExists = await _db.Dishes.AnyAsync(d => d.Id == dishId);
            if (!dishExists)
                return NotFound(BaseApiResponse<object>.ErrorResult($"Dish with ID {dishId} was not found.", new[] { "Dish not found" }));

            var media = await _db.MediaFiles.FirstOrDefaultAsync(m => m.Id == request.MediaFileId);
            if (media == null)
                return BadRequest(BaseApiResponse<object>.ErrorResult("Invalid MediaFileId", new[] { "Media file not found" }));

            var role = string.IsNullOrWhiteSpace(request.Role) ? "gallery" : request.Role.Trim().ToLowerInvariant();
            if (role != "cover" && role != "gallery")
                return BadRequest(BaseApiResponse<object>.ErrorResult("Invalid role", new[] { "Role must be cover or gallery" }));

            if (role == "cover")
            {
                var existingCovers = await _db.DishImages.Where(x => x.DishId == dishId && x.Role == "cover").ToListAsync();
                foreach (var c in existingCovers) c.Role = "gallery";
            }

            var entity = new SmartLunch.Backend.Service.Domain.Entities.DishImage
            {
                DishId = dishId,
                MediaFileId = request.MediaFileId,
                Role = role,
                SortOrder = request.SortOrder ?? 0,
                CreatedAt = VietnamTime.Now
            };

            _db.DishImages.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(BaseApiResponse<object>.SuccessResult(new { Id = entity.Id }, "Dish image added"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding dish image. DishId={DishId}", dishId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("An error occurred while adding dish image", new[] { ex.Message }));
        }
    }

    [HttpDelete("{dishImageId:int}")]
    [Authorize(Policy = "permission:dishes.update")]
    public async Task<ActionResult<BaseApiResponse<object>>> DeleteImage(int dishId, int dishImageId)
    {
        try
        {
            var entity = await _db.DishImages.FirstOrDefaultAsync(x => x.Id == dishImageId && x.DishId == dishId);
            if (entity == null)
                return NotFound(BaseApiResponse<object>.NotFoundResult("Dish image not found"));

            _db.DishImages.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok(BaseApiResponse<object>.SuccessResult(new { }, "Dish image deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dish image. DishId={DishId}, DishImageId={DishImageId}", dishId, dishImageId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("An error occurred while deleting dish image", new[] { ex.Message }));
        }
    }
}

public sealed class AddDishImageRequest
{
    public int MediaFileId { get; set; }
    public string? Role { get; set; } // cover | gallery
    public int? SortOrder { get; set; }
}

