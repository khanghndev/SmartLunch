using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Users;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Users.CreateUser;

public record CreateUserCommand(CreateUserRequest Request, Guid ActorUserId) : IRequest<GetUserResponse>;
