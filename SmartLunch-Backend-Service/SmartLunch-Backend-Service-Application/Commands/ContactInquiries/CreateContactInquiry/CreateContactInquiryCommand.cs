using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;

namespace SmartLunch.Backend.Service.Application.Commands.ContactInquiries.CreateContactInquiry;

public sealed record CreateContactInquiryCommand(CreateContactInquiryRequest Request, int? UserId = null)
    : IRequest<CreateContactInquiryResponse>;
