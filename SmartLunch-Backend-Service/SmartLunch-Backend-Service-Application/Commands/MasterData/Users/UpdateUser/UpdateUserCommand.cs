using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Users;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Users.UpdateUser;

public record UpdateUserCommand(Guid UserId, UpdateUserRequest Request, Guid ActorUserId) : IRequest<GetUserResponse>;
