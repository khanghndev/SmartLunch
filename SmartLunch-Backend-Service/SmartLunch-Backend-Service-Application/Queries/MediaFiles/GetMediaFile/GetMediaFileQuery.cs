using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MediaFiles;

namespace SmartLunch.Backend.Service.Application.Queries.MediaFiles.GetMediaFile;

public class GetMediaFileQuery : IRequest<GetMediaFileResponse>
{
    public Guid MediaFileId { get; set; }

    public GetMediaFileQuery(Guid mediaFileId)
    {
        MediaFileId = mediaFileId;
    }
}
