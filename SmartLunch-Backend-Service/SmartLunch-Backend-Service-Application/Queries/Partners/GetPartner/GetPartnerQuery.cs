using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

namespace SmartLunch.Backend.Service.Application.Queries.Partners.GetPartner;

/// <summary>
/// Query to get a partner by ID
/// </summary>
public class GetPartnerQuery : IRequest<GetPartnerResponse>
{
    public Guid PartnerId { get; set; }

    /// <summary>Trả kèm danh sách hợp đồng (thời gian cung cấp theo từng hợp đồng).</summary>
    public bool IncludeContracts { get; set; } = true;

    public GetPartnerQuery(Guid partnerId, bool includeContracts = true)
    {
        PartnerId = partnerId;
        IncludeContracts = includeContracts;
    }
}
