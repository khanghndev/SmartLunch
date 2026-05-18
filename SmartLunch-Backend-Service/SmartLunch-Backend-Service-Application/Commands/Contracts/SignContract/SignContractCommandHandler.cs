using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.SignContract;

public class SignContractCommandHandler : IRequestHandler<SignContractCommand, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractPdfService _contractPdfService;

    public SignContractCommandHandler(
        IContractRepository contractRepository,
        IContractPdfService contractPdfService)
    {
        _contractRepository = contractRepository;
        _contractPdfService = contractPdfService;
    }

    public async Task<GetContractResponse> Handle(SignContractCommand request, CancellationToken cancellationToken)
    {
        var sig = (request.Request.DigitalSignature ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(sig))
            throw new ArgumentException("DigitalSignature is required.");

        var entity = await _contractRepository.GetByIdAsync(request.ContractId);
        if (entity == null)
            throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        if (entity.IsDigitallySigned)
            throw new InvalidOperationException("Contract is already digitally signed.");

        entity.IsDigitallySigned = true;
        entity.DigitalSignature = sig;
        entity.DigitallySignedAt = VietnamTime.Now;
        entity.SignatureImage = string.IsNullOrWhiteSpace(request.Request.SignatureImageUrl)
            ? null
            : request.Request.SignatureImageUrl.Trim();
        entity.UpdatedAt = VietnamTime.Now;

        await _contractRepository.UpdateAsync(entity);

        var signed = await _contractRepository.GetByIdAsync(request.ContractId);
        if (signed?.Partner == null)
            throw new InvalidOperationException("Partner not loaded for contract PDF.");

        var pdfUrl = await _contractPdfService.GenerateUploadAndResolveUrlAsync(
            signed,
            signed.Partner,
            signed.Organization,
            cancellationToken);

        signed.ContractFileUrl = pdfUrl;
        signed.UpdatedAt = VietnamTime.Now;
        await _contractRepository.UpdateAsync(signed);

        var reloaded = await _contractRepository.GetByIdAsync(entity.Id);
        return new GetContractResponse { Contract = ContractDtoMapping.ToDto(reloaded ?? signed) };
    }
}
