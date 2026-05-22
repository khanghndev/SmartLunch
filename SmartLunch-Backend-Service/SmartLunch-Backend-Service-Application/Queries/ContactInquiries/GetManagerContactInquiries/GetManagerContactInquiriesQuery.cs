using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.ContactInquiries;

namespace SmartLunch.Backend.Service.Application.Queries.ContactInquiries.GetManagerContactInquiries;

public sealed record GetManagerContactInquiriesQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    string? Status = null) : IRequest<GetManagerContactInquiriesResponse>;
