using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserTokens;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserTokens.GetUserTokens;

public class GetUserTokensQueryHandler : IRequestHandler<GetUserTokensQuery, GetUserTokensResponse>
{
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly ILogger<GetUserTokensQueryHandler> _logger;

    public GetUserTokensQueryHandler(IUserTokenRepository userTokenRepository, ILogger<GetUserTokensQueryHandler> logger)
    {
        _userTokenRepository = userTokenRepository;
        _logger = logger;
    }

    public async Task<GetUserTokensResponse> Handle(GetUserTokensQuery request, CancellationToken cancellationToken)
    {
        var (userTokens, totalCount) = await _userTokenRepository.GetUserTokensAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive);

        var userTokenDtos = userTokens.Select(userToken => new UserTokenDto
        {
                Id = userToken.Id,
                UserId = userToken.UserId,
                IssuedAt = userToken.IssuedAt,
                ExpiresAt = userToken.ExpiresAt,
                RevokedAt = userToken.RevokedAt,
                IsActive = userToken.IsActive
        }).ToList();

        _logger.LogInformation("Retrieved {Count} usertokens (Page {Page}, PageSize {PageSize})",
            userTokenDtos.Count, request.Page, request.PageSize);

        return new GetUserTokensResponse
        {
            Data = userTokenDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
