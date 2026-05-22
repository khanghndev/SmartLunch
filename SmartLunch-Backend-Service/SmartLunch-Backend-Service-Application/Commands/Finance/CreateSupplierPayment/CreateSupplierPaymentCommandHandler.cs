using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.Finance.CreateSupplierPayment;

public sealed class CreateSupplierPaymentCommandHandler : IRequestHandler<CreateSupplierPaymentCommand, CreateSupplierPaymentResponse>
{
    private static readonly HashSet<string> AllowedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "bank_transfer", "cash", "card"
    };

    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        PartnerPaymentStatus.Pending,
        PartnerPaymentStatus.Completed,
        PartnerPaymentStatus.Failed,
    };

    private readonly IContractRepository _contracts;
    private readonly IPartnerPaymentRepository _payments;

    public CreateSupplierPaymentCommandHandler(IContractRepository contracts, IPartnerPaymentRepository payments)
    {
        _contracts = contracts;
        _payments = payments;
    }

    public async Task<CreateSupplierPaymentResponse> Handle(CreateSupplierPaymentCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.ContractId <= 0 || req.PartnerId <= 0)
            throw new ArgumentException("ContractId và PartnerId là bắt buộc.");
        if (req.Amount <= 0)
            throw new ArgumentException("Số tiền phải lớn hơn 0.");

        var method = string.IsNullOrWhiteSpace(req.Method) ? "bank_transfer" : req.Method.Trim().ToLowerInvariant();
        if (!AllowedMethods.Contains(method))
            throw new ArgumentException("Phương thức thanh toán không hợp lệ.");

        var status = string.IsNullOrWhiteSpace(req.Status) ? PartnerPaymentStatus.Completed : req.Status.Trim().ToLowerInvariant();
        if (!AllowedStatuses.Contains(status))
            throw new ArgumentException("Trạng thái thanh toán không hợp lệ.");

        var contract = await _contracts.GetByIdAsync(req.ContractId);
        if (contract == null)
            throw new KeyNotFoundException($"Contract not found: {req.ContractId}");
        if (contract.PartnerId != req.PartnerId)
            throw new ArgumentException("Hợp đồng không thuộc nhà cung cấp đã chọn.");
        if (string.Equals(contract.Status, ContractStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Không thể thanh toán cho hợp đồng đã hủy.");

        if (contract.TotalValue.HasValue && status == PartnerPaymentStatus.Completed)
        {
            var paidOnContract = await _payments.GetCompletedTotalByContractAsync(req.ContractId, cancellationToken);
            var remaining = contract.TotalValue.Value - paidOnContract;
            if (req.Amount > remaining + 0.01m)
                throw new ArgumentException(
                    $"Số tiền vượt phần còn lại của hợp đồng ({remaining:N0} đ).");
        }

        var paymentDate = req.PaymentDate == default ? VietnamTime.Now : req.PaymentDate;

        var entity = new PartnerPayment
        {
            ContractId = req.ContractId,
            PartnerId = req.PartnerId,
            PaymentDate = paymentDate,
            Amount = req.Amount,
            Method = method,
            Status = status,
            CreatedAt = VietnamTime.Now,
        };

        var created = await _payments.CreateAsync(entity, cancellationToken);

        return new CreateSupplierPaymentResponse
        {
            Id = created.Id,
            Code = created.Code,
            ContractId = created.ContractId,
            PartnerId = created.PartnerId,
            Amount = created.Amount,
            Status = created.Status,
        };
    }
}
