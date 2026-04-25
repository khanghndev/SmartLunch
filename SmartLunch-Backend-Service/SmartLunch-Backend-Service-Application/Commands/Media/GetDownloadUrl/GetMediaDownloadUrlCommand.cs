using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;

namespace SmartLunch.Backend.Service.Application.Commands.Media.GetDownloadUrl;

public class GetMediaDownloadUrlCommand : IRequest<GetMediaDownloadUrlResponse>
{
    public int UserId { get; }
    public int MediaFileId { get; }
    public int? ExpiresMinutes { get; }

    public GetMediaDownloadUrlCommand(int userId, int mediaFileId, int? expiresMinutes = null)
    {
        UserId = userId;
        MediaFileId = mediaFileId;
        ExpiresMinutes = expiresMinutes;
    }
}

