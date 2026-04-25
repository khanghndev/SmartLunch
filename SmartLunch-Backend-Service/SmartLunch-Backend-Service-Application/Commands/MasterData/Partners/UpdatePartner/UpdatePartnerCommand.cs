using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Partners;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Partners.UpdatePartner;

public class UpdatePartnerCommand : IRequest<GetPartnerResponse>
{
    public int PartnerId { get; }
    public UpdatePartnerRequest Request { get; }

    public UpdatePartnerCommand(int partnerId, UpdatePartnerRequest request)
    {
        PartnerId = partnerId;
        Request = request;
    }
}
