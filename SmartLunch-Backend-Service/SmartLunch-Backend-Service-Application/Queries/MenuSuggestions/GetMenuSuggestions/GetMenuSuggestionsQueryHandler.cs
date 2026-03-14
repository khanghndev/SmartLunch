using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSuggestions.GetMenuSuggestions;

public class GetMenuSuggestionsQueryHandler : IRequestHandler<GetMenuSuggestionsQuery, GetMenuSuggestionsResponse>
{
    private readonly IMenuSuggestionRepository _menuSuggestionRepository;
    private readonly ILogger<GetMenuSuggestionsQueryHandler> _logger;

    public GetMenuSuggestionsQueryHandler(IMenuSuggestionRepository menuSuggestionRepository, ILogger<GetMenuSuggestionsQueryHandler> logger)
    {
        _menuSuggestionRepository = menuSuggestionRepository;
        _logger = logger;
    }

    public async Task<GetMenuSuggestionsResponse> Handle(GetMenuSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var (menuSuggestions, totalCount) = await _menuSuggestionRepository.GetMenuSuggestionsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var menuSuggestionDtos = menuSuggestions.Select(menuSuggestion => new MenuSuggestionDto
        {
                Id = menuSuggestion.Id,
                WeekStart = menuSuggestion.WeekStart,
                GeneratedAt = menuSuggestion.GeneratedAt,
                SuggestionText = menuSuggestion.SuggestionText,
                AlgorithmVersion = menuSuggestion.AlgorithmVersion,
                CreatedBy = menuSuggestion.CreatedBy
        }).ToList();

        _logger.LogInformation("Retrieved {Count} menusuggestions (Page {Page}, PageSize {PageSize})",
            menuSuggestionDtos.Count, request.Page, request.PageSize);

        return new GetMenuSuggestionsResponse
        {
            Data = menuSuggestionDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
