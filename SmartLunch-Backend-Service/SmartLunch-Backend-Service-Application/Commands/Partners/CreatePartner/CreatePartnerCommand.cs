using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Partners;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Partners;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Partners.CreatePartner;

public class CreatePartnerCommand : IRequest<GetPartnerResponse>
{
    public CreatePartnerRequest Request { get; }

    public CreatePartnerCommand(CreatePartnerRequest request)
    {
        Request = request;
    }
}
