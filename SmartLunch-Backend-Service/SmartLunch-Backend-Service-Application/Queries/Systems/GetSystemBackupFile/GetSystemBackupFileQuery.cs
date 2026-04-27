using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemBackupFile;

public class GetSystemBackupFileQuery : IRequest<GetSystemBackupFileResponse>
{
    public int BackupId { get; }

    public GetSystemBackupFileQuery(int backupId)
    {
        BackupId = backupId;
    }
}

