using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Queries.Auth.GetUserProfile;

public class GetUserProfileQuery : IRequest<UserProfileResponse>
{
    public int UserId { get; }

    public GetUserProfileQuery(int userId)
    {
        UserId = userId;
    }
}
