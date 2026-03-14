using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

namespace SmartLunch.Backend.Service.Application.Queries.Partners.GetPartner;

/// <summary>
/// Query to get a partner by ID
/// </summary>
public class GetPartnerQuery : IRequest<GetPartnerResponse>
{
    public Guid PartnerId { get; set; }

    public GetPartnerQuery(Guid partnerId)
    {
        PartnerId = partnerId;
    }
}
