using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.SignContract;

public class SignContractCommandHandler : IRequestHandler<SignContractCommand, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IContractPdfService _contractPdfService;
    private readonly IOrganizationMealDocumentPdfService _combinedPdfService;
    private readonly IOrderRepository _orderRepository;

    public SignContractCommandHandler(
        IContractRepository contractRepository,
        IContractPdfService contractPdfService,
        IOrganizationMealDocumentPdfService combinedPdfService,
        IOrderRepository orderRepository)
    {
        _contractRepository = contractRepository;
        _contractPdfService = contractPdfService;
        _combinedPdfService = combinedPdfService;
        _orderRepository = orderRepository;
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

        string pdfUrl;
        if (signed.SourceOrderId is int orderId &&
            signed.Organization != null &&
            await _orderRepository.GetByIdWithDetailsAsync(orderId) is Order linkedOrder &&
            linkedOrder.Contract?.Partner != null)
        {
            pdfUrl = await _combinedPdfService.GenerateCombinedUploadAndResolveUrlAsync(
                signed,
                signed.Partner,
                signed.Organization,
                linkedOrder,
                signed.Organization.Name,
                sig,
                cancellationToken);
            linkedOrder.AnnexPdfUrl = pdfUrl;
            linkedOrder.UpdatedAt = VietnamTime.Now;
            await _orderRepository.CommitAsync();
        }
        else
        {
            pdfUrl = await _contractPdfService.GenerateUploadAndResolveUrlAsync(
                signed,
                signed.Partner,
                signed.Organization,
                cancellationToken: cancellationToken);
        }

        signed.ContractFileUrl = pdfUrl;
        signed.UpdatedAt = VietnamTime.Now;
        await _contractRepository.UpdateAsync(signed);

        var reloaded = await _contractRepository.GetByIdAsync(entity.Id);
        return new GetContractResponse { Contract = ContractDtoMapping.ToDto(reloaded ?? signed) };
    }
}
