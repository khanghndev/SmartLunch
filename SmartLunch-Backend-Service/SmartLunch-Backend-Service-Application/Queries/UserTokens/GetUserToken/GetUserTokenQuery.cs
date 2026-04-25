using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserTokens;

namespace SmartLunch.Backend.Service.Application.Queries.UserTokens.GetUserToken;

public class GetUserTokenQuery : IRequest<GetUserTokenResponse>
{
    public int UserTokenId { get; set; }

    public GetUserTokenQuery(int userTokenId)
    {
        UserTokenId = userTokenId;
    }
}
