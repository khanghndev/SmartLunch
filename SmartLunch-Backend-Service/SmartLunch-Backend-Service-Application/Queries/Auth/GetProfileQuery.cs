using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Auth;

namespace SmartLunch.Backend.Service.Application.Queries.Auth;

public record GetProfileQuery(int UserId) : IRequest<UserProfileResponse>;
