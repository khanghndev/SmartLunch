using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class DishCoverImageSync
{
    public static async Task ApplyCoverObjectNameAsync(
        Dish entity,
        IReadOnlyList<UpdateDishImageItemRequest>? images,
        IMediaFileRepository mediaFiles,
        CancellationToken cancellationToken = default)
    {
        if (images == null || images.Count == 0)
            return;

        var coverReq = images
            .OrderByDescending(i => string.Equals(i.Role, "cover", StringComparison.OrdinalIgnoreCase))
            .ThenBy(i => i.SortOrder ?? 0)
            .FirstOrDefault();

        if (coverReq == null)
            return;

        var media = await mediaFiles.GetByIdAsync(coverReq.MediaFileId);
        if (media != null && !string.IsNullOrWhiteSpace(media.ObjectName))
            entity.ImageUrl = media.ObjectName.Trim();
    }
}
