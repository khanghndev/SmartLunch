using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.MenuSuggestions;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

namespace SmartLunch.Backend.Service.Application.Commands.MenuSuggestions.CreateMenuSuggestion;

public class CreateMenuSuggestionCommand : IRequest<CreateMenuSuggestionResponse>
{
    public CreateMenuSuggestionRequest Request { get; }
    public int? CreatedByUserId { get; }

    public CreateMenuSuggestionCommand(CreateMenuSuggestionRequest request, int? createdByUserId)
    {
        Request = request;
        CreatedByUserId = createdByUserId;
    }
}

