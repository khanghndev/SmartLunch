using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Systems;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

namespace SmartLunch.Backend.Service.Application.Commands.Systems.UpdateBackupSchedule;

public record UpdateBackupScheduleCommand(UpdateBackupScheduleRequest Request, int ActorUserId)
    : IRequest<BackupScheduleDto>;
