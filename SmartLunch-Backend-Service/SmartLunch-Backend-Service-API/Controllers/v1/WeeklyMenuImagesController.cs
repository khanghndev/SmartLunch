using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Infrastructure.Data;
using System.Net;

namespace SmartLunch.Backend.Service.API.Controllers.MasterData;

/// <summary>
/// WeeklyMenu images management (cover + gallery).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/master-data/WeeklyMenu/{weeklyMenuId:int}/images")]
[Authorize(Policy = "roles:Admin,Manager")]
public class WeeklyMenuImagesController : ControllerBase
{
    private readonly ILogger<WeeklyMenuImagesController> _logger;
    private readonly SmartLunchDBContext _db;

    public WeeklyMenuImagesController(ILogger<WeeklyMenuImagesController> logger, SmartLunchDBContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost]
    [Authorize(Policy = "permission:weekly_menus.update")]
    public async Task<ActionResult<BaseApiResponse<object>>> AddImage(int weeklyMenuId, [FromBody] AddWeeklyMenuImageRequest request)
    {
        try
        {
            var menuExists = await _db.WeeklyMenus.AnyAsync(m => m.Id == weeklyMenuId);
            if (!menuExists)
                return NotFound(BaseApiResponse<object>.ErrorResult($"WeeklyMenu with ID {weeklyMenuId} was not found.", new[] { "WeeklyMenu not found" }));

            var media = await _db.MediaFiles.FirstOrDefaultAsync(m => m.Id == request.MediaFileId);
            if (media == null)
                return BadRequest(BaseApiResponse<object>.ErrorResult("Invalid MediaFileId", new[] { "Media file not found" }));

            var role = string.IsNullOrWhiteSpace(request.Role) ? "gallery" : request.Role.Trim().ToLowerInvariant();
            if (role != "cover" && role != "gallery")
                return BadRequest(BaseApiResponse<object>.ErrorResult("Invalid role", new[] { "Role must be cover or gallery" }));

            if (role == "cover")
            {
                var existingCovers = await _db.WeeklyMenuImages.Where(x => x.WeeklyMenuId == weeklyMenuId && x.Role == "cover").ToListAsync();
                foreach (var c in existingCovers) c.Role = "gallery";
            }

            var entity = new SmartLunch.Backend.Service.Domain.Entities.WeeklyMenuImage
            {
                WeeklyMenuId = weeklyMenuId,
                MediaFileId = request.MediaFileId,
                Role = role,
                SortOrder = request.SortOrder ?? 0,
                CreatedAt = VietnamTime.Now
            };

            _db.WeeklyMenuImages.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(BaseApiResponse<object>.SuccessResult(new { entity.Id }, "WeeklyMenu image added"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding weekly menu image. WeeklyMenuId={WeeklyMenuId}", weeklyMenuId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("An error occurred while adding weekly menu image", new[] { ex.Message }));
        }
    }

    [HttpDelete("{weeklyMenuImageId:int}")]
    [Authorize(Policy = "permission:weekly_menus.update")]
    public async Task<ActionResult<BaseApiResponse<object>>> DeleteImage(int weeklyMenuId, int weeklyMenuImageId)
    {
        try
        {
            var entity = await _db.WeeklyMenuImages.FirstOrDefaultAsync(x => x.Id == weeklyMenuImageId && x.WeeklyMenuId == weeklyMenuId);
            if (entity == null)
                return NotFound(BaseApiResponse<object>.NotFoundResult("WeeklyMenu image not found"));

            _db.WeeklyMenuImages.Remove(entity);
            await _db.SaveChangesAsync();
            return Ok(BaseApiResponse<object>.SuccessResult(new { }, "WeeklyMenu image deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting weekly menu image. WeeklyMenuId={WeeklyMenuId}, WeeklyMenuImageId={WeeklyMenuImageId}", weeklyMenuId, weeklyMenuImageId);
            return StatusCode((int)HttpStatusCode.InternalServerError,
                BaseApiResponse<object>.ErrorResult("An error occurred while deleting weekly menu image", new[] { ex.Message }));
        }
    }
}

public sealed class AddWeeklyMenuImageRequest
{
    public int MediaFileId { get; set; }
    public string? Role { get; set; } // cover | gallery
    public int? SortOrder { get; set; }
}

