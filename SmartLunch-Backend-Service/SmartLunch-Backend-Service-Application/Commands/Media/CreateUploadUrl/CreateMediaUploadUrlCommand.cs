using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Media;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;

namespace SmartLunch.Backend.Service.Application.Commands.Media.CreateUploadUrl;

public class CreateMediaUploadUrlCommand : IRequest<CreateMediaUploadUrlResponse>
{
    public Guid UserId { get; }
    public CreateMediaUploadUrlRequest Request { get; }

    public CreateMediaUploadUrlCommand(Guid userId, CreateMediaUploadUrlRequest request)
    {
        UserId = userId;
        Request = request;
    }
}

