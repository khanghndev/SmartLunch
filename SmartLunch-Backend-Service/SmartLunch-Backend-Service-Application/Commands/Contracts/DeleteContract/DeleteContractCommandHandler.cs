using MediatR;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.DeleteContract;

public class DeleteContractCommandHandler : IRequestHandler<DeleteContractCommand, DeleteContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly ICacheService _cacheService;

    public DeleteContractCommandHandler(IContractRepository contractRepository, ICacheService cacheService)
    {
        _contractRepository = contractRepository;
        _cacheService = cacheService;
    }

    public async Task<DeleteContractResponse> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var entity = await _contractRepository.GetByIdAsync(request.ContractId);
        if (entity == null)
            throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        var partnerId = entity.PartnerId;
        var paymentCount = await _contractRepository.CountPartnerPaymentsAsync(request.ContractId);

        if (paymentCount > 0)
        {
            entity.Status = ContractStatus.Cancelled;
            entity.UpdatedAt = VietnamTime.Now;
            await _contractRepository.UpdateAsync(entity);
            await _cacheService.RemoveAsync(MasterDataCacheKeys.Partner(partnerId), cancellationToken);
            return new DeleteContractResponse
            {
                Id = entity.Id,
                Message = "Contract has partner payments; status set to cancelled instead of removing."
            };
        }

        await _contractRepository.DeleteAsync(entity);
        await _cacheService.RemoveAsync(MasterDataCacheKeys.Partner(partnerId), cancellationToken);

        return new DeleteContractResponse
        {
            Id = request.ContractId,
            Message = "Contract deleted successfully."
        };
    }
}
