using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Mappings;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.CreateActualIntakeFromProposal;

public class CreateActualIntakeFromProposalCommandHandler
    : IRequestHandler<CreateActualIntakeFromProposalCommand, CreateActualIntakeFromProposalResponse>
{
    private readonly IIngredientActualIntakeRepository _actualIntakeRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<CreateActualIntakeFromProposalCommandHandler> _logger;

    public CreateActualIntakeFromProposalCommandHandler(
        IIngredientActualIntakeRepository actualIntakeRepository,
        IUserRoleRepository userRoleRepository,
        ILogger<CreateActualIntakeFromProposalCommandHandler> logger)
    {
        _actualIntakeRepository = actualIntakeRepository;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<CreateActualIntakeFromProposalResponse> Handle(
        CreateActualIntakeFromProposalCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Request.ConfirmIngredientsMeetStandard != true)
        {
            throw new ArgumentException(
                "Phải xác nhận nguyên liệu đạt chuẩn: gửi confirmIngredientsMeetStandard = true.");
        }

        var userRoles = await _userRoleRepository.GetActiveByUserIdAsync(request.ActorUserId);
        var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();
        if (!IntakeProposalAccessHelper.CanCreateIntakeProposal(roleNames))
            throw new UnauthorizedAccessException("You are not allowed to record actual intake.");

        var elevated = IntakeProposalAccessHelper.IsElevatedReviewer(roleNames);
        var receivedAt = request.Request.ReceivedAtUtc ?? DateTime.UtcNow;
        if (receivedAt.Kind == DateTimeKind.Unspecified)
            receivedAt = DateTime.SpecifyKind(receivedAt, DateTimeKind.Utc);

        var receipt = await _actualIntakeRepository.CreateFromApprovedProposalAsync(
            request.ProposalId,
            request.ActorUserId,
            elevated,
            receivedAt,
            request.Request.Note,
            cancellationToken);

        _logger.LogInformation(
            "Actual intake {ReceiptCode} created for proposal {ProposalId}",
            receipt.ReceiptCode,
            request.ProposalId);

        return new CreateActualIntakeFromProposalResponse
        {
            Receipt = IngredientActualIntakeMapping.ToDetail(receipt)
        };
    }
}
