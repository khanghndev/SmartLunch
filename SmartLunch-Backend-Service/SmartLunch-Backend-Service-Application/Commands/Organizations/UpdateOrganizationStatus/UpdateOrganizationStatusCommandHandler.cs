using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Organizations;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.Organizations.UpdateOrganizationStatus;

public class UpdateOrganizationStatusCommandHandler : IRequestHandler<UpdateOrganizationStatusCommand, GetOrganizationResponse>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ICacheService _cacheService;

    public UpdateOrganizationStatusCommandHandler(
        IOrganizationRepository organizationRepository,
        ICacheService cacheService)
    {
        _organizationRepository = organizationRepository;
        _cacheService = cacheService;
    }

    public async Task<GetOrganizationResponse> Handle(UpdateOrganizationStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _organizationRepository.GetByIdAsync(request.OrganizationId);
        if (entity == null)
            throw new KeyNotFoundException($"Organization with ID {request.OrganizationId} was not found.");

        entity.IsActive = request.IsActive;
        entity.UpdatedAt = VietnamTime.Now;

        await _organizationRepository.UpdateAsync(entity, cancellationToken);

        await _cacheService.RemoveAsync(MasterDataCacheKeys.Organization(request.OrganizationId), cancellationToken);

        return new GetOrganizationResponse
        {
            Organization = MapDto(entity)
        };
    }

    private static OrganizationDto MapDto(Organization u) => new()
    {
        Id = u.Id,
        Code = u.Code,
        Name = u.Name,
        Address = u.Address,
        Phone = u.Phone,
        ContactPerson = u.ContactPerson,
        ContactEmail = u.ContactEmail,
        TaxCode = u.TaxCode,
        LegalRepresentative = u.LegalRepresentative,
        LogoUrl = u.LogoUrl,
        Website = u.Website,
        EducationLevel = u.EducationLevel,
        Type = u.Type,
        IsSubscriptionActive = u.IsSubscriptionActive,
        DefaultDailyMeals = u.DefaultDailyMeals,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };
}
