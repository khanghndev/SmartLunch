using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSuggestions.GetMenuSuggestion;

public class GetMenuSuggestionQueryHandler : IRequestHandler<GetMenuSuggestionQuery, GetMenuSuggestionResponse>
{
    private readonly IMenuSuggestionRepository _menuSuggestionRepository;
    private readonly ILogger<GetMenuSuggestionQueryHandler> _logger;

    public GetMenuSuggestionQueryHandler(IMenuSuggestionRepository menuSuggestionRepository, ILogger<GetMenuSuggestionQueryHandler> logger)
    {
        _menuSuggestionRepository = menuSuggestionRepository;
        _logger = logger;
    }

    public async Task<GetMenuSuggestionResponse> Handle(GetMenuSuggestionQuery request, CancellationToken cancellationToken)
    {
        var menuSuggestion = await _menuSuggestionRepository.GetByIdAsync(request.MenuSuggestionId);

        if (menuSuggestion == null)
        {
            _logger.LogWarning("MenuSuggestion not found with ID: {MenuSuggestionId}", request.MenuSuggestionId);
            return new GetMenuSuggestionResponse { MenuSuggestion = new MenuSuggestionDto() };
        }

        return new GetMenuSuggestionResponse
        {
            MenuSuggestion = new MenuSuggestionDto
            {
                Id = menuSuggestion.Id,
                WeekStart = menuSuggestion.WeekStart,
                GeneratedAt = menuSuggestion.GeneratedAt,
                SuggestionText = menuSuggestion.SuggestionText,
                AlgorithmVersion = menuSuggestion.AlgorithmVersion,
                CreatedBy = menuSuggestion.CreatedBy
            }
        };
    }
}
