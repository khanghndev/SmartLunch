using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.ContactInquiries.CreateContactInquiry;

public sealed class CreateContactInquiryCommandHandler : IRequestHandler<CreateContactInquiryCommand, CreateContactInquiryResponse>
{
    private readonly IContactInquiryRepository _repository;

    public CreateContactInquiryCommandHandler(IContactInquiryRepository repository) => _repository = repository;

    public async Task<CreateContactInquiryResponse> Handle(CreateContactInquiryCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (string.IsNullOrWhiteSpace(req.FullName))
            throw new ArgumentException("Họ tên là bắt buộc.");
        if (string.IsNullOrWhiteSpace(req.Phone))
            throw new ArgumentException("Số điện thoại là bắt buộc.");
        if (string.IsNullOrWhiteSpace(req.Email))
            throw new ArgumentException("Email là bắt buộc.");
        if (!req.Email.Contains('@'))
            throw new ArgumentException("Email không hợp lệ.");

        var entity = new ContactInquiry
        {
            UserId = request.UserId,
            FullName = req.FullName.Trim(),
            Phone = req.Phone.Trim(),
            Email = req.Email.Trim(),
            InterestedService = string.IsNullOrWhiteSpace(req.InterestedService) ? "Khác" : req.InterestedService.Trim(),
            Message = string.IsNullOrWhiteSpace(req.Message) ? null : req.Message.Trim(),
            Status = "pending",
            CreatedAt = VietnamTime.Now,
        };

        var created = await _repository.CreateAsync(entity, cancellationToken);

        return new CreateContactInquiryResponse
        {
            Id = created.Id,
            Code = created.Code,
        };
    }
}
