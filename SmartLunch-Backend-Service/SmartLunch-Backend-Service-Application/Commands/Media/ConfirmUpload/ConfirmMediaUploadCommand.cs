using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Media;
using SmartLunch.Backend.Service.Application.DTOs.Response.Media;

namespace SmartLunch.Backend.Service.Application.Commands.Media.ConfirmUpload;

public class ConfirmMediaUploadCommand : IRequest<ConfirmMediaUploadResponse>
{
    public int UserId { get; }
    public ConfirmMediaUploadRequest Request { get; }

    public ConfirmMediaUploadCommand(int userId, ConfirmMediaUploadRequest request)
    {
        UserId = userId;
        Request = request;
    }
}

