using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetBackupSchedule;

public class GetBackupScheduleQuery : IRequest<BackupScheduleDto>
{
}
