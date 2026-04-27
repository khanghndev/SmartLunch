using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemLogs;

public class GetSystemLogsQueryHandler : IRequestHandler<GetSystemLogsQuery, GetSystemLogsResponse>
{
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly ILogger<GetSystemLogsQueryHandler> _logger;

    public GetSystemLogsQueryHandler(ISystemLogRepository systemLogRepository, ILogger<GetSystemLogsQueryHandler> logger)
    {
        _systemLogRepository = systemLogRepository;
        _logger = logger;
    }

    public async Task<GetSystemLogsResponse> Handle(GetSystemLogsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 10 : request.PageSize;

        var (logs, totalCount) = await _systemLogRepository.GetSystemLogsAsync(page, pageSize);

        var dtos = logs.Select(e => new SystemLogDto
        {
            Id = e.Id,
            Timestamp = e.Timestamp,
            Level = e.Level,
            Template = e.Template,
            Message = e.Message,
            Exception = e.Exception,
            Properties = e.Properties
        }).ToList();

        _logger.LogInformation("Retrieved {Count} system logs (Page {Page}, PageSize {PageSize})",
            dtos.Count, page, pageSize);

        return new GetSystemLogsResponse
        {
            Data = dtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}

