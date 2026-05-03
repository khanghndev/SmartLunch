using MediatR;
using Microsoft.Extensions.Configuration;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using System.Net.Http;

namespace SmartLunch.Backend.Service.Application.Queries.Auth;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;

    public GetProfileQueryHandler(IUserRepository userRepository, IStorageService storage, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _storage = storage;
        _configuration = configuration;
    }

    public async Task<UserProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var response = new UserProfileResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = await ResolveAvatarUrlAsync(user),
            Address = user.Address,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        // Get the first unit the user belongs to (if any)
        var userUnit = user.UserUnits.FirstOrDefault(uu => uu.IsActive);
        if (userUnit != null && userUnit.Unit != null)
        {
            response.Unit = new UnitInfoResponse
            {
                Id = userUnit.Unit.Id,
                Name = userUnit.Unit.Name,
                TaxCode = userUnit.Unit.TaxCode,
                LegalRepresentative = userUnit.Unit.LegalRepresentative,
                Address = userUnit.Unit.Address,
                Phone = userUnit.Unit.Phone,
                ContactEmail = userUnit.Unit.ContactEmail,
                UnitType = userUnit.Unit.UnitType
            };
        }

        return response;
    }

    private async Task<string?> ResolveAvatarUrlAsync(User user)
    {
        var raw = user.AvatarUrl;
        if (string.IsNullOrWhiteSpace(raw)) return null;

        if (raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return raw;
        }

        // Avatar is treated as public image when uploaded via /auth/profile/avatar.
        // If bucket isn't public, this will still work via signed URL.
        var isPublic = user.AvatarMediaFileId.HasValue;
        if (isPublic)
        {
            var endpoint = (_configuration["Appwrite:Endpoint"] ?? "https://syd.cloud.appwrite.io/v1").TrimEnd('/');
            var bucketId = _configuration["Appwrite:BucketId"] ?? "";
            var projectId = _configuration["Appwrite:ProjectId"] ?? "";
            var fileId = ToFileId(raw);
            return $"{endpoint}/storage/buckets/{bucketId}/files/{Uri.EscapeDataString(fileId)}/view?project={Uri.EscapeDataString(projectId)}";
        }

        var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
        var signed = await _storage.CreateSignedUrlAsync(raw, HttpMethod.Get, contentType: null, expiresIn: expiresIn);
        return signed.Url;
    }

    private static string ToFileId(string objectName)
    {
        var normalized = objectName.Trim();
        var hash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(normalized));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return $"f_{hex[..34]}";
    }
}
