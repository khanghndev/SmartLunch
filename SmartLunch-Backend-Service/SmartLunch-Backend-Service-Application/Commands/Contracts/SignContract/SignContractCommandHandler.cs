using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.SignContract;

public class SignContractCommandHandler : IRequestHandler<SignContractCommand, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;

    public SignContractCommandHandler(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<GetContractResponse> Handle(SignContractCommand request, CancellationToken cancellationToken)
    {
        var sig = (request.Request.DigitalSignature ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(sig))
            throw new ArgumentException("DigitalSignature is required.");

        var entity = await _contractRepository.GetByIdAsync(request.ContractId);
        if (entity == null)
            throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        // Idempotent: if already signed, do not allow overwrite by default
        if (entity.IsDigitallySigned)
            throw new InvalidOperationException("Contract is already digitally signed.");

        entity.IsDigitallySigned = true;
        entity.DigitalSignature = sig;
        entity.DigitallySignedAt = DateTime.UtcNow;
        entity.SignatureImage = string.IsNullOrWhiteSpace(request.Request.SignatureImageUrl)
            ? null
            : request.Request.SignatureImageUrl.Trim();
        entity.UpdatedAt = DateTime.UtcNow;

        await _contractRepository.UpdateAsync(entity);

        var reloaded = await _contractRepository.GetByIdAsync(entity.Id);
        return new GetContractResponse { Contract = ContractDtoMapping.ToDto(reloaded ?? entity) };
    }
}

