using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationMealOrders;

/// <summary>Ghi hợp đồng đặt suất đơn vị vào bảng contracts khi lập / render hợp đồng (Prepare).</summary>
public sealed class OrganizationMealContractDraftPersistence
{
    private readonly IContractRepository _contractRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IContractPdfService _contractPdfService;

    public OrganizationMealContractDraftPersistence(
        IContractRepository contractRepository,
        IPartnerRepository partnerRepository,
        IOrganizationRepository organizationRepository,
        IContractPdfService contractPdfService)
    {
        _contractRepository = contractRepository;
        _partnerRepository = partnerRepository;
        _organizationRepository = organizationRepository;
        _contractPdfService = contractPdfService;
    }

    public async Task<Contract> EnsurePersistedAsync(
        OrganizationMealOrderDraftPayload draft,
        CancellationToken cancellationToken)
    {
        var org = await _organizationRepository.GetByIdAsync(draft.OrganizationId);
        if (org == null || !org.IsActive)
            throw new ArgumentException("Organization is not available.");

        Contract? contract = null;
        if (draft.ContractId > 0)
            contract = await _contractRepository.GetByIdAsync(draft.ContractId);

        contract ??= await _contractRepository.GetActiveForOrganizationAsync(draft.OrganizationId, cancellationToken);

        var orderedDays = draft.Days.OrderBy(d => d.ServiceDate).ToList();
        var minDate = orderedDays.Min(d => d.ServiceDate);
        var maxDate = orderedDays.Max(d => d.ServiceDate);
        var supplySchedule = BuildSupplySchedule(orderedDays);
        var description = BuildDescription(org, draft, minDate, maxDate);
        var wasNew = false;

        if (contract == null)
        {
            var (partners, _) = await _partnerRepository.GetPartnersAsync(
                page: 1,
                pageSize: 1,
                searchTerm: null,
                isActive: true);

            var partner = partners.FirstOrDefault()
                ?? throw new InvalidOperationException("No active partner found to create contract.");

            contract = new Contract
            {
                PartnerId = partner.Id,
                OrganizationId = org.Id,
                ContractType = "Order-Based",
                ContractNumber = await AllocateContractNumberAsync(partner.Id, org.Id, minDate, cancellationToken),
                Description = description,
                SupplySchedule = supplySchedule,
                StartDate = minDate.ToDateTime(TimeOnly.MinValue),
                EndDate = maxDate.ToDateTime(TimeOnly.MinValue),
                TotalValue = draft.TotalAmount,
                MealUnitPrice = draft.PricePerPortion,
                DepositAmount = null,
                Status = ContractStatus.Active,
                CreatedAt = VietnamTime.Now,
            };
            contract = await _contractRepository.CreateAsync(contract);
            wasNew = true;
        }
        else
        {
            contract.MealUnitPrice = draft.PricePerPortion;
            if (string.Equals(contract.ContractType, "Order-Based", StringComparison.OrdinalIgnoreCase))
            {
                contract.TotalValue = draft.TotalAmount;
                contract.Description = description;
                contract.SupplySchedule = supplySchedule;
                contract.StartDate = minDate.ToDateTime(TimeOnly.MinValue);
                contract.EndDate = maxDate.ToDateTime(TimeOnly.MinValue);
            }

            contract.UpdatedAt = VietnamTime.Now;
            await _contractRepository.UpdateAsync(contract);
        }

        draft.ContractId = contract.Id;

        if (wasNew || string.IsNullOrWhiteSpace(contract.ContractFileUrl))
            contract = await GenerateAndStorePdfAsync(contract.Id, org, cancellationToken);

        return contract;
    }

    private async Task<Contract> GenerateAndStorePdfAsync(
        int contractId,
        Organization org,
        CancellationToken cancellationToken)
    {
        var forPdf = await _contractRepository.GetByIdAsync(contractId)
            ?? throw new InvalidOperationException("Contract not found after persist.");

        if (forPdf.Partner == null)
            return forPdf;

        var url = await _contractPdfService.GenerateUploadAndResolveUrlAsync(
            forPdf,
            forPdf.Partner,
            org,
            cancellationToken);

        forPdf.ContractFileUrl = url;
        forPdf.UpdatedAt = VietnamTime.Now;
        await _contractRepository.UpdateAsync(forPdf);
        return forPdf;
    }

    private static string BuildDescription(
        Organization org,
        OrganizationMealOrderDraftPayload draft,
        DateOnly minDate,
        DateOnly maxDate)
    {
        var contact = string.Join(" · ", new[]
        {
            org.ContactPerson,
            org.Phone,
            org.ContactEmail
        }.Where(s => !string.IsNullOrWhiteSpace(s)));

        return $"Hợp đồng đặt suất — {org.Name} ({minDate:dd/MM/yyyy}–{maxDate:dd/MM/yyyy}). " +
               $"{draft.TotalMainQuantity} suất chính, đơn giá {draft.PricePerPortion:N0} đ/suất, tổng {draft.TotalAmount:N0} đ." +
               (string.IsNullOrEmpty(contact) ? "" : $" Liên hệ: {contact}.");
    }

    private static string BuildSupplySchedule(IReadOnlyList<OrganizationMealOrderDraftDay> days)
    {
        var dates = days.Select(d => d.ServiceDate.ToString("dd/MM/yyyy")).Distinct();
        return "Ngày phục vụ: " + string.Join(", ", dates);
    }

    private async Task<string> AllocateContractNumberAsync(
        int partnerId,
        int organizationId,
        DateOnly refDate,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 12; attempt++)
        {
            var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            var candidate = $"HD-B2B-{organizationId}-{refDate:yyyyMMdd}-{suffix}";
            if (!await _contractRepository.ExistsContractNumberForPartnerAsync(partnerId, candidate))
                return candidate;
        }

        throw new InvalidOperationException("Could not allocate a unique contract number.");
    }
}
