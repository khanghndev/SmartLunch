using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

namespace SmartLunch.Backend.Service.Application.Commands.Systems.DeleteSystemBackup;

public class DeleteSystemBackupCommand : IRequest<DeleteSystemBackupResponse>
{
    public int BackupId { get; }
    public bool DeletePhysicalFile { get; }
    public int ActorUserId { get; }

    public DeleteSystemBackupCommand(int backupId, bool deletePhysicalFile, int actorUserId)
    {
        BackupId = backupId;
        DeletePhysicalFile = deletePhysicalFile;
        ActorUserId = actorUserId;
    }
}

