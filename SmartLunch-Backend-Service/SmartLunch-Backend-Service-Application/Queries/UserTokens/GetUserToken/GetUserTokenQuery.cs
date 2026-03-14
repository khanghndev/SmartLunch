using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserTokens;

namespace SmartLunch.Backend.Service.Application.Queries.UserTokens.GetUserToken;

public class GetUserTokenQuery : IRequest<GetUserTokenResponse>
{
    public Guid UserTokenId { get; set; }

    public GetUserTokenQuery(Guid userTokenId)
    {
        UserTokenId = userTokenId;
    }
}
