using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.GenerateIngredientPrepFromAi;

public sealed record GenerateIngredientPrepFromAiCommand(
    GenerateIngredientPrepFromAiRequest Request,
    int ActorUserId
) : IRequest<GenerateIngredientPrepFromAiResponse>;

