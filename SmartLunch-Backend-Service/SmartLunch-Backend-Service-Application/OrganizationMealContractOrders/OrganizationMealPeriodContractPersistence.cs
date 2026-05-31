using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationMealContractOrders;

public sealed class OrganizationMealPeriodContractPersistence
{
    private readonly IContractRepository _contractRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IContractPdfService _contractPdfService;

    public OrganizationMealPeriodContractPersistence(
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
        OrganizationMealPeriodContractDraftPayload draft,
        CancellationToken cancellationToken)
    {
        var org = await _organizationRepository.GetByIdAsync(draft.OrganizationId);
        if (org == null || !org.IsActive)
            throw new ArgumentException("Organization is not available.");

        Contract? contract = null;
        if (draft.ContractId > 0)
            contract = await _contractRepository.GetPeriodBasedWithExcludedDatesAsync(draft.ContractId, cancellationToken);

        if (contract != null && (contract.IsDigitallySigned || contract.SourceOrderId.HasValue))
            contract = null;

        var description = BuildDescription(org, draft);
        var totalMeals = draft.TotalMeals > 0
            ? draft.TotalMeals
            : OrganizationMealPeriodContractCalculator.CountTotalMeals(
                draft.StartDate,
                draft.EndDate,
                draft.ExcludedDates,
                draft.MealsPerDay,
                draft.DailyMealOverrides);
        var supplySchedule = $"Thời hạn {draft.StartDate:dd/MM/yyyy}–{draft.EndDate:dd/MM/yyyy}; " +
                             $"{totalMeals} suất ({draft.ServiceDays} ngày phục vụ, mặc định {draft.MealsPerDay} suất/ngày).";

        if (contract == null)
        {
            var (partners, _) = await _partnerRepository.GetPartnersAsync(1, 1, null, true);
            var partner = partners.FirstOrDefault()
                ?? throw new InvalidOperationException("No active partner found to create contract.");

            contract = new Contract
            {
                PartnerId = partner.Id,
                OrganizationId = org.Id,
                ContractType = OrganizationMealContractTypes.PeriodBased,
                ContractNumber = await AllocateContractNumberAsync(partner.Id, org.Id, draft.StartDate, cancellationToken),
                Description = description,
                SupplySchedule = supplySchedule,
                StartDate = draft.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = draft.EndDate.ToDateTime(TimeOnly.MinValue),
                TotalValue = draft.TotalAmount,
                MealUnitPrice = draft.MealUnitPrice,
                MealsPerDay = draft.MealsPerDay,
                DepositAmount = null,
                Status = ContractStatus.Active,
                CreatedAt = VietnamTime.Now,
            };
            contract = await _contractRepository.CreateAsync(contract);
            await _contractRepository.ReplaceExcludedDatesAsync(contract.Id, draft.ExcludedDates, cancellationToken);
            await PersistDailyMealPortionsAsync(contract.Id, draft, cancellationToken);
        }
        else
        {
            contract.MealUnitPrice = draft.MealUnitPrice;
            contract.MealsPerDay = draft.MealsPerDay;
            contract.TotalValue = draft.TotalAmount;
            contract.Description = description;
            contract.SupplySchedule = supplySchedule;
            contract.StartDate = draft.StartDate.ToDateTime(TimeOnly.MinValue);
            contract.EndDate = draft.EndDate.ToDateTime(TimeOnly.MinValue);
            contract.UpdatedAt = VietnamTime.Now;
            await _contractRepository.UpdateAsync(contract);
            await _contractRepository.ReplaceExcludedDatesAsync(contract.Id, draft.ExcludedDates, cancellationToken);
            await PersistDailyMealPortionsAsync(contract.Id, draft, cancellationToken);
        }

        draft.ContractId = contract.Id;

        if (string.IsNullOrWhiteSpace(contract.ContractFileUrl))
            contract = await GenerateAndStorePdfAsync(contract.Id, org, draft, cancellationToken);

        return await _contractRepository.GetPeriodBasedWithExcludedDatesAsync(contract.Id, cancellationToken)
            ?? contract;
    }

    private async Task<Contract> GenerateAndStorePdfAsync(
        int contractId,
        Organization org,
        OrganizationMealPeriodContractDraftPayload draft,
        CancellationToken cancellationToken)
    {
        var forPdf = await _contractRepository.GetByIdAsync(contractId)
            ?? throw new InvalidOperationException("Contract not found after persist.");

        if (forPdf.Partner == null)
            return forPdf;

        var deliveryPdf = OrganizationMealOrders.OrganizationMealDeliveryPdfContext.FromDraft(draft.Delivery);
        var url = await _contractPdfService.GenerateUploadAndResolveUrlAsync(
            forPdf,
            forPdf.Partner,
            org,
            order: null,
            delivery: deliveryPdf,
            cancellationToken: cancellationToken);

        forPdf.ContractFileUrl = url;
        forPdf.UpdatedAt = VietnamTime.Now;
        await _contractRepository.UpdateAsync(forPdf);
        return forPdf;
    }

    private static string BuildDescription(Organization org, OrganizationMealPeriodContractDraftPayload draft)
    {
        var excluded = draft.ExcludedDates.Count == 0
            ? "không có ngày nghỉ"
            : $"{draft.ExcludedDates.Count} ngày không cung cấp";

        var totalMeals = draft.TotalMeals > 0
            ? draft.TotalMeals
            : OrganizationMealPeriodContractCalculator.CountTotalMeals(
                draft.StartDate,
                draft.EndDate,
                draft.ExcludedDates,
                draft.MealsPerDay,
                draft.DailyMealOverrides);
        var customDays = draft.DailyMealOverrides.Count;
        var customNote = customDays > 0 ? $", {customDays} ngày tùy chỉnh số suất" : "";

        return $"Hợp đồng đặt suất theo kỳ — {org.Name} ({draft.StartDate:dd/MM/yyyy}–{draft.EndDate:dd/MM/yyyy}). " +
               $"{totalMeals} suất / {draft.ServiceDays} ngày phục vụ (mặc định {draft.MealsPerDay} suất/ngày{customNote}, {excluded}), " +
               $"đơn giá {draft.MealUnitPrice:N0} đ/suất, tổng {draft.TotalAmount:N0} đ.";
    }

    private async Task PersistDailyMealPortionsAsync(
        int contractId,
        OrganizationMealPeriodContractDraftPayload draft,
        CancellationToken cancellationToken)
    {
        var portions = draft.DailyMealOverrides
            .Select(kv => new ContractDailyMealPortionSource(kv.Key, kv.Value))
            .ToList();
        await _contractRepository.ReplaceDailyMealPortionsAsync(contractId, portions, cancellationToken);
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
            var candidate = $"HD-KY-{organizationId}-{refDate:yyyyMMdd}-{suffix}";
            if (!await _contractRepository.ExistsContractNumberForPartnerAsync(partnerId, candidate))
                return candidate;
        }

        throw new InvalidOperationException("Could not allocate a unique contract number.");
    }
}
