using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.MenuSuggestions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MenuSuggestions.CreateMenuSuggestion;

public class CreateMenuSuggestionCommandHandler : IRequestHandler<CreateMenuSuggestionCommand, CreateMenuSuggestionResponse>
{
    private readonly IMenuSuggestionRepository _menuSuggestionRepository;
    private readonly ILogger<CreateMenuSuggestionCommandHandler> _logger;

    public CreateMenuSuggestionCommandHandler(
        IMenuSuggestionRepository menuSuggestionRepository,
        ILogger<CreateMenuSuggestionCommandHandler> logger)
    {
        _menuSuggestionRepository = menuSuggestionRepository;
        _logger = logger;
    }

    public async Task<CreateMenuSuggestionResponse> Handle(
        CreateMenuSuggestionCommand request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;

        if (req == null)
            throw new ArgumentException("Request is required.");

        if (req.WeekStart == default)
            throw new ArgumentException("WeekStart is required.");

        if (string.IsNullOrWhiteSpace(req.SuggestionText))
            throw new ArgumentException("SuggestionText is required.");

        var trimmed = req.SuggestionText.Trim();
        if (trimmed.Length == 0)
            throw new ArgumentException("SuggestionText is required.");

        var algorithmVersion = string.IsNullOrWhiteSpace(req.AlgorithmVersion) ? null : req.AlgorithmVersion.Trim();
        if (algorithmVersion != null && algorithmVersion.Length > 50)
            throw new ArgumentException("AlgorithmVersion max length is 50.");

        // Persist date-only semantics; DB column is DATE.
        var weekStartUtc = DateTime.SpecifyKind(req.WeekStart.Date, DateTimeKind.Utc);

        var entity = new MenuSuggestion
        {
            WeekStart = weekStartUtc,
            GeneratedAt = DateTime.UtcNow,
            SuggestionText = trimmed,
            AlgorithmVersion = algorithmVersion,
            CreatedBy = request.CreatedByUserId,
        };

        await _menuSuggestionRepository.AddAsync(entity, cancellationToken);
        await _menuSuggestionRepository.CommitAsync();

        _logger.LogInformation("Created menu suggestion for week starting {WeekStart}", weekStartUtc);

        return new CreateMenuSuggestionResponse
        {
            MenuSuggestion = new MenuSuggestionDto
            {
                WeekStart = entity.WeekStart,
                GeneratedAt = entity.GeneratedAt,
                SuggestionText = entity.SuggestionText,
                AlgorithmVersion = entity.AlgorithmVersion,
                CreatedBy = entity.CreatedBy
            }
        };
    }
}

