using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.BackupSystem;

public class BackupSystemQuery : IRequest<BackupSystemResponse>
{
    public int Id { get; set; }

    public BackupSystemQuery(int id)
    {
        Id = id;
    }
}

