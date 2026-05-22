using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;

namespace SmartLunch.Backend.Service.Application.Commands.ContactInquiries.ReplyToContactInquiry;

public sealed record ReplyToContactInquiryCommand(int InquiryId, int ManagerUserId, string Reply)
    : IRequest<ManagerContactInquiryListItemDto>;
