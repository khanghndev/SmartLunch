using MediatR;
using SmartLunch.Backend.Service.Application.ContactInquiries;
using SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.ContactInquiries.GetManagerContactInquiries;

public sealed class GetManagerContactInquiriesQueryHandler
    : IRequestHandler<GetManagerContactInquiriesQuery, GetManagerContactInquiriesResponse>
{
    private readonly IContactInquiryRepository _repository;

    public GetManagerContactInquiriesQueryHandler(IContactInquiryRepository repository) => _repository = repository;

    public async Task<GetManagerContactInquiriesResponse> Handle(
        GetManagerContactInquiriesQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total, pending) = await _repository.GetManagerListAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            cancellationToken);

        return new GetManagerContactInquiriesResponse
        {
            Data = items.Select(ContactInquiryMapper.ToManagerDto).ToList(),
            TotalCount = total,
            PendingCount = pending,
            Page = request.Page,
            PageSize = request.PageSize,
        };
    }
}
