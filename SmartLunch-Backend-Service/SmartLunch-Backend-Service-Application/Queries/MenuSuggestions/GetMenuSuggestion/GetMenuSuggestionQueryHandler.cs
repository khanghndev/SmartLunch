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
                Version = menuSuggestion.Version,
                RulesKey = menuSuggestion.RulesKey,
                BudgetPerServing = menuSuggestion.BudgetPerServing,
                TopK = menuSuggestion.TopK,
                TimeLimitSeconds = menuSuggestion.TimeLimitSeconds,
                PlanCount = menuSuggestion.PlanCount,
                AlgorithmVersion = menuSuggestion.AlgorithmVersion,
                CreatedBy = menuSuggestion.CreatedBy,
                Plans = menuSuggestion.Plans
                    .OrderBy(p => p.Rank)
                    .Select(p => new MenuSuggestionPlanDto
                    {
                        Id = p.Id,
                        Rank = p.Rank,
                        PlanScore = p.PlanScore,
                        ObjectiveValue = p.ObjectiveValue,
                        Days = p.Days
                            .OrderBy(d => d.DayIndex)
                            .Select(d => new MenuSuggestionPlanDayDto
                            {
                                Id = d.Id,
                                DayIndex = d.DayIndex,
                                DayName = d.DayName,
                                Items = d.Items
                                    .Select(i =>
                                    {
                                        var itemDto = new MenuSuggestionPlanItemDto
                                        {
                                            Id = i.Id,
                                            SlotCategory = i.SlotCategory,
                                            DishId = i.DishId,
                                            DishName = i.DishName,
                                            DishSourceCategory = i.DishSourceCategory,
                                            Score = i.Score,
                                            CostPerServing = i.CostPerServing,
                                        };
                                        itemDto.SetReasonsFromJson(i.ReasonsJson);
                                        return itemDto;
                                    })
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            }
        };
    }
}
