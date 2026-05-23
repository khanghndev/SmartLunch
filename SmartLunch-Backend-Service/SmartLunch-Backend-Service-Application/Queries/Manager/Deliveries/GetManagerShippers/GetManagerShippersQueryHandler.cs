using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Manager.Deliveries.GetManagerShippers;

public class GetManagerShippersQueryHandler : IRequestHandler<GetManagerShippersQuery, GetManagerShippersResponse>
{
    private readonly IUserRepository _userRepository;

    public GetManagerShippersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetManagerShippersResponse> Handle(GetManagerShippersQuery request, CancellationToken cancellationToken)
    {
        var (users, _) = await _userRepository.GetUsersAsync(1, 200, null, isActive: true, roleName: "Shipper", staffOnly: null);

        var shippers = users.Select(u =>
        {
            var name = $"{u.FirstName} {u.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(name)) name = u.Username;
            return new ManagerShipperOptionDto
            {
                UserId = u.Id,
                DisplayName = name,
                PhoneNumber = u.PhoneNumber,
                IsActive = u.IsActive,
            };
        }).OrderBy(s => s.DisplayName).ToList();

        return new GetManagerShippersResponse { Shippers = shippers };
    }
}
