using SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.ContactInquiries;

public static class ContactInquiryMapper
{
    public static ManagerContactInquiryListItemDto ToManagerDto(ContactInquiry entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        FullName = entity.FullName,
        Phone = entity.Phone,
        Email = entity.Email,
        InterestedService = entity.InterestedService,
        Message = entity.Message,
        Status = entity.Status,
        ManagerReply = entity.ManagerReply,
        RepliedAt = entity.RepliedAt,
        CreatedAt = entity.CreatedAt,
    };
}
