using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using System.Net.Http;

namespace SmartLunch.Backend.Service.Application.Commands.Media.GetDownloadUrl;

public class GetMediaDownloadUrlCommandHandler : IRequestHandler<GetMediaDownloadUrlCommand, GetMediaDownloadUrlResponse>
{
    private readonly IMediaFileRepository _mediaFileRepository;
    private readonly IFirebaseStorageService _storage;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetMediaDownloadUrlCommandHandler> _logger;

    public GetMediaDownloadUrlCommandHandler(
        IMediaFileRepository mediaFileRepository,
        IFirebaseStorageService storage,
        IConfiguration configuration,
        ILogger<GetMediaDownloadUrlCommandHandler> logger)
    {
        _mediaFileRepository = mediaFileRepository;
        _storage = storage;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<GetMediaDownloadUrlResponse> Handle(GetMediaDownloadUrlCommand command, CancellationToken cancellationToken)
    {
        var media = await _mediaFileRepository.GetByIdForOwnerAsync(command.MediaFileId, command.UserId);
        if (media == null)
        {
            throw new KeyNotFoundException("Media file not found");
        }

        var defaultExpires = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresMinutes = command.ExpiresMinutes ?? defaultExpires;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 120));

        var signed = await _storage.CreateSignedUrlAsync(
            media.ObjectName,
            HttpMethod.Get,
            contentType: null,
            expiresIn: expiresIn,
            requiredHeaders: null);

        _logger.LogInformation(
            "Issued signed download URL. UserId={UserId}, MediaFileId={MediaFileId}, ObjectName={ObjectName}",
            command.UserId, command.MediaFileId, media.ObjectName);

        return new GetMediaDownloadUrlResponse
        {
            DownloadUrl = signed.Url,
            ExpiresAtUtc = signed.ExpiresAtUtc
        };
    }
}

