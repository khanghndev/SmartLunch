using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

/// <summary>Kết quả load hợp đồng + đơn + thanh toán NCC để báo cáo theo hợp đồng.</summary>
public sealed record ContractFinanceSettlementSnapshot(
    Contract Contract,
    IReadOnlyList<Order> Orders,
    IReadOnlyList<PartnerPayment> SupplierPayments);
