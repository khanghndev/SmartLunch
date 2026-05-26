using MediatR;
using SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.InitiateOrganizationMealPayment;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.InitiateOrganizationMealPeriodPayment;

public sealed class InitiateOrganizationMealPeriodPaymentCommandHandler
    : IRequestHandler<InitiateOrganizationMealPeriodPaymentCommand, InitiateOrganizationMealPaymentResponse>
{
    private readonly IMediator _mediator;
    private readonly IOrderRepository _orderRepository;
    private readonly IContractRepository _contractRepository;

    public InitiateOrganizationMealPeriodPaymentCommandHandler(
        IMediator mediator,
        IOrderRepository orderRepository,
        IContractRepository contractRepository)
    {
        _mediator = mediator;
        _orderRepository = orderRepository;
        _contractRepository = contractRepository;
    }

    public async Task<InitiateOrganizationMealPaymentResponse> Handle(
        InitiateOrganizationMealPeriodPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(command.Request.OrderId);
        if (order?.ContractId is not int contractId)
            throw new KeyNotFoundException($"Order {command.Request.OrderId} was not found.");

        var contract = await _contractRepository.GetByIdAsync(contractId);
        if (contract == null ||
            !string.Equals(contract.ContractType, OrganizationMealContractTypes.PeriodBased, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Order is not linked to a Period-Based meal contract.");
        }

        if (order.UserId != command.UserId)
            throw new UnauthorizedAccessException("You do not have access to this order.");

        return await _mediator.Send(
            new InitiateOrganizationMealPaymentCommand(
                command.UserId,
                new InitiateOrganizationMealPaymentRequest
                {
                    OrderId = command.Request.OrderId,
                    ReturnUrl = command.Request.ReturnUrl,
                    CancelUrl = command.Request.CancelUrl,
                }),
            cancellationToken);
    }
}
