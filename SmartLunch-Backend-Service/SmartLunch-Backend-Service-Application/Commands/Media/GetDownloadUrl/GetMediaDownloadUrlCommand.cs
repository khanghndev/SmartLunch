using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;

namespace SmartLunch.Backend.Service.Application.Commands.Media.GetDownloadUrl;

public class GetMediaDownloadUrlCommand : IRequest<GetMediaDownloadUrlResponse>
{
    public Guid UserId { get; }
    public Guid MediaFileId { get; }
    public int? ExpiresMinutes { get; }

    public GetMediaDownloadUrlCommand(Guid userId, Guid mediaFileId, int? expiresMinutes = null)
    {
        UserId = userId;
        MediaFileId = mediaFileId;
        ExpiresMinutes = expiresMinutes;
    }
}

