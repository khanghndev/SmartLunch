using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Partners.CreatePartner;

public class CreatePartnerCommandHandler : IRequestHandler<CreatePartnerCommand, GetPartnerResponse>
{
    private readonly IPartnerRepository _partnerRepository;

    public CreatePartnerCommandHandler(IPartnerRepository partnerRepository)
    {
        _partnerRepository = partnerRepository;
    }

    public async Task<GetPartnerResponse> Handle(CreatePartnerCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (string.IsNullOrWhiteSpace(req.LegalName))
            throw new ArgumentException("LegalName is required.");

        if (!string.IsNullOrWhiteSpace(req.TaxId) &&
            await _partnerRepository.ExistsByTaxIdAsync(req.TaxId.Trim()))
            throw new InvalidOperationException($"A partner with tax ID '{req.TaxId.Trim()}' already exists.");

        var entity = new Partner
        {

            LegalName = req.LegalName.Trim(),
            BusinessRegistrationNumber = string.IsNullOrWhiteSpace(req.BusinessRegistrationNumber)
                ? null
                : req.BusinessRegistrationNumber.Trim(),
            TaxId = string.IsNullOrWhiteSpace(req.TaxId) ? null : req.TaxId.Trim(),
            LegalRepresentative = string.IsNullOrWhiteSpace(req.LegalRepresentative)
                ? null
                : req.LegalRepresentative.Trim(),
            Address = req.Address?.Trim(),
            ContactPerson = req.ContactPerson?.Trim(),
            Phone = req.Phone?.Trim(),
            Email = req.Email?.Trim(),
            PerformanceRating = req.PerformanceRating,
            ComplianceInfo = req.ComplianceInfo?.Trim(),
            FinancialTerms = req.FinancialTerms?.Trim(),
            IsActive = req.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _partnerRepository.CreateAsync(entity);

        var reloaded = await _partnerRepository.GetByIdWithContractsAsync(entity.Id);
        if (reloaded == null)
            return new GetPartnerResponse { Partner = PartnerDtoMapping.ToDto(entity) };

        return new GetPartnerResponse
        {
            Partner = PartnerDtoMapping.ToDto(reloaded),
            Contracts = reloaded.Contracts
                .OrderByDescending(c => c.StartDate)
                .Select(PartnerDtoMapping.ToContractSummary)
                .ToList()
        };
    }
}
