using MediatR;
using SmartLunch.Backend.Service.Application.ContactInquiries;
using SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.ContactInquiries.ReplyToContactInquiry;

public sealed class ReplyToContactInquiryCommandHandler
    : IRequestHandler<ReplyToContactInquiryCommand, ManagerContactInquiryListItemDto>
{
    private readonly IContactInquiryRepository _repository;

    public ReplyToContactInquiryCommandHandler(IContactInquiryRepository repository) => _repository = repository;

    public async Task<ManagerContactInquiryListItemDto> Handle(
        ReplyToContactInquiryCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Reply))
            throw new ArgumentException("Nội dung trả lời là bắt buộc.");

        var entity = await _repository.GetByIdAsync(request.InquiryId, cancellationToken);
        if (entity == null)
            throw new KeyNotFoundException($"Contact inquiry not found with ID: {request.InquiryId}");

        entity.ManagerReply = request.Reply.Trim();
        entity.RepliedAt = VietnamTime.Now;
        entity.RepliedByUserId = request.ManagerUserId;
        entity.Status = "replied";

        await _repository.UpdateAsync(entity, cancellationToken);

        return ContactInquiryMapper.ToManagerDto(entity);
    }
}
