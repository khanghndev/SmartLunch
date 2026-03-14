using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserTokens;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.UserTokens.GetUserToken;

public class GetUserTokenQueryHandler : IRequestHandler<GetUserTokenQuery, GetUserTokenResponse>
{
    private readonly IUserTokenRepository _userTokenRepository;
    private readonly ILogger<GetUserTokenQueryHandler> _logger;

    public GetUserTokenQueryHandler(IUserTokenRepository userTokenRepository, ILogger<GetUserTokenQueryHandler> logger)
    {
        _userTokenRepository = userTokenRepository;
        _logger = logger;
    }

    public async Task<GetUserTokenResponse> Handle(GetUserTokenQuery request, CancellationToken cancellationToken)
    {
        var userToken = await _userTokenRepository.GetByIdAsync(request.UserTokenId);

        if (userToken == null)
        {
            _logger.LogWarning("UserToken not found with ID: {UserTokenId}", request.UserTokenId);
            return new GetUserTokenResponse { UserToken = new UserTokenDto() };
        }

        return new GetUserTokenResponse
        {
            UserToken = new UserTokenDto
            {
                Id = userToken.Id,
                UserId = userToken.UserId,
                IssuedAt = userToken.IssuedAt,
                ExpiresAt = userToken.ExpiresAt,
                RevokedAt = userToken.RevokedAt,
                IsActive = userToken.IsActive
            }
        };
    }
}
