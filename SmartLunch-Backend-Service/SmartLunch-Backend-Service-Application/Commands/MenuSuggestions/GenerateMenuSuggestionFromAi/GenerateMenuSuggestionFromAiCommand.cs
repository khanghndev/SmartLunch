using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

namespace SmartLunch.Backend.Service.Application.Commands.MenuSuggestions.GenerateMenuSuggestionFromAi;

public class GenerateMenuSuggestionFromAiCommand : IRequest<CreateMenuSuggestionResponse>
{
    public GenerateMenuSuggestionFromAiRequest Request { get; }
    public int? CreatedByUserId { get; }

    public GenerateMenuSuggestionFromAiCommand(GenerateMenuSuggestionFromAiRequest request, int? createdByUserId)
    {
        Request = request;
        CreatedByUserId = createdByUserId;
    }
}

