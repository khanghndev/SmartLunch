using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.DTOs.Request.Shipper.Deliveries;
using SmartLunch.Backend.Service.Application.DTOs.Response.Shipper.Deliveries;
using SmartLunch.Backend.Service.Application.Commands.Shipper.Deliveries.UpdateShipperDeliveryStatus;
using SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Deliveries.GetShipperDeliveries;
using SmartLunch.Backend.Service.Application.Queries.ShipperFeatures.Deliveries.GetShipperDelivery;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.API.Controllers.v1;

/// <summary>
/// Delivery tasks for Shipper role.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shipper/deliveries")]
[Authorize(Policy = "roles:Shipper")]
public class ShipperDeliveryController : ControllerBase
{
    private readonly ILogger<ShipperDeliveryController> _logger;
    private readonly IMediator _mediator;
    private readonly IStorageService _storage;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IDeliveryHandoverPdfService _handoverPdfService;

    public ShipperDeliveryController(
        ILogger<ShipperDeliveryController> logger,
        IMediator mediator,
        IStorageService storage,
        IDeliveryRepository deliveryRepository,
        IDeliveryHandoverPdfService handoverPdfService)
    {
        _logger = logger;
        _mediator = mediator;
        _storage = storage;
        _deliveryRepository = deliveryRepository;
        _handoverPdfService = handoverPdfService;
    }

    /// <summary>
    /// Xem danh sách đơn cần giao (task của shipper).
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "permission:deliveries.list")]
    public async Task<ActionResult<BaseApiResponse<GetShipperDeliveriesResponse>>> GetDeliveries(
        [FromQuery] GetShipperDeliveriesRequest request)
    {
        try
        {
            var shipperId = RequireUserId();
            var query = new GetShipperDeliveriesQuery(
                shipperId,
                request.Page,
                request.PageSize,
                request.Status,
                request.ScheduledOn);
            var response = await _mediator.Send(query);
            return Ok(BaseApiResponse<GetShipperDeliveriesResponse>.SuccessResult(response, "Deliveries retrieved successfully"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetShipperDeliveriesResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shipper deliveries");
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShipperDeliveriesResponse>.ErrorResult("An error occurred while retrieving deliveries", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Xem chi tiết đơn cần giao (địa điểm, số suất, thời gian giao).
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = "permission:deliveries.read")]
    public async Task<ActionResult<BaseApiResponse<GetShipperDeliveryResponse>>> GetDelivery(int id)
    {
        try
        {
            var shipperId = RequireUserId();
            var response = await _mediator.Send(new GetShipperDeliveryQuery(shipperId, id));
            return Ok(BaseApiResponse<GetShipperDeliveryResponse>.SuccessResult(response, "Delivery retrieved successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetShipperDeliveryResponse>.NotFoundResult(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shipper delivery {DeliveryId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("An error occurred while retrieving delivery", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Cập nhật trạng thái giao hàng: received/picking_up → in_transit → completed/failed, hoặc reject.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = "permission:deliveries.update")]
    public async Task<ActionResult<BaseApiResponse<GetShipperDeliveryResponse>>> UpdateStatus(
        int id,
        [FromBody] UpdateShipperDeliveryStatusRequest request)
    {
        try
        {
            var shipperId = RequireUserId();
            var response = await _mediator.Send(new UpdateShipperDeliveryStatusCommand(shipperId, id, request));
            return Ok(BaseApiResponse<GetShipperDeliveryResponse>.SuccessResult(response, "Delivery status updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(BaseApiResponse<GetShipperDeliveryResponse>.NotFoundResult(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating delivery status {DeliveryId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("An error occurred while updating delivery status", new[] { ex.Message }));
        }
    }

    /// <summary>
    /// Xác nhận giao hàng: ảnh PoD + chữ ký shipper + chữ ký người nhận → sinh PDF biên bản bàn giao.
    /// </summary>
    [HttpPost("{id:int}/proof")]
    [Authorize(Policy = "permission:deliveries.update")]
    [RequestSizeLimit(15 * 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<GetShipperDeliveryResponse>>> UploadProof(
        int id,
        [FromForm] UploadDeliveryProofForm form)
    {
        try
        {
            var shipperId = RequireUserId();
            if (form == null)
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("Form is required", new[] { "Form data is missing." }));

            var file = form.File;
            var signature = form.Signature;
            var shipperSignature = form.ShipperSignature;

            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("File is required", new[] { "Missing proof photo." }));

            if (signature == null || signature.Length <= 0)
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("Signature is required", new[] { "Missing recipient signature." }));

            if (shipperSignature == null || shipperSignature.Length <= 0)
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("ShipperSignature is required", new[] { "Missing shipper signature." }));

            if (string.IsNullOrWhiteSpace(form.RecipientConfirmedName))
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("RecipientConfirmedName is required", new[] { "Missing recipient name." }));

            var delivery = await _deliveryRepository.GetByIdWithOrderAsync(id, HttpContext.RequestAborted);
            if (delivery == null)
                return NotFound(BaseApiResponse<GetShipperDeliveryResponse>.NotFoundResult($"Delivery with ID {id} was not found."));

            if (delivery.AssignedStaffId.HasValue && delivery.AssignedStaffId.Value != shipperId)
                return NotFound(BaseApiResponse<GetShipperDeliveryResponse>.NotFoundResult("Delivery was not found."));

            if (!delivery.AssignedStaffId.HasValue)
                return Conflict(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("Delivery is not assigned to you yet.", new[] { "Receive the delivery first." }));

            if (!string.Equals(delivery.DeliveryStatus, "in_transit", StringComparison.OrdinalIgnoreCase))
                return Conflict(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult(
                    "Delivery must be in_transit before proof upload.",
                    new[] { $"Current status: {delivery.DeliveryStatus}" }));

            var now = VietnamTime.Now;
            byte[] proofBytes;
            byte[] signatureBytes;
            byte[] shipperSignatureBytes;
            string proofContentType;
            try
            {
                proofContentType = file.ContentType ?? "image/jpeg";
                await using (var proofStream = new MemoryStream())
                {
                    await file.CopyToAsync(proofStream, HttpContext.RequestAborted);
                    proofBytes = proofStream.ToArray();
                }
                await using (var sigStream = new MemoryStream())
                {
                    await signature.CopyToAsync(sigStream, HttpContext.RequestAborted);
                    signatureBytes = sigStream.ToArray();
                }
                await using (var shipperSigStream = new MemoryStream())
                {
                    await shipperSignature.CopyToAsync(shipperSigStream, HttpContext.RequestAborted);
                    shipperSignatureBytes = shipperSigStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult(
                    "Could not read uploaded files.",
                    new[] { ex.Message }));
            }

            string proofUrl;
            string signatureUrl;
            string shipperSignatureUrl;
            try
            {
                proofUrl = await UploadDeliveryImageBytesAsync(
                    proofBytes, proofContentType, id, "proof", now, HttpContext.RequestAborted);
                signatureUrl = await UploadDeliveryImageBytesAsync(
                    signatureBytes, "image/png", id, "signature", now, HttpContext.RequestAborted);
                shipperSignatureUrl = await UploadDeliveryImageBytesAsync(
                    shipperSignatureBytes, "image/png", id, "shipper-signature", now, HttpContext.RequestAborted);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }

            var recipientName = form.RecipientConfirmedName.Trim();
            var notes = string.IsNullOrWhiteSpace(form.Notes) ? null : form.Notes.Trim();
            var mealCount = delivery.Order?.OrderItems?.Sum(i => i.Quantity) ?? 0;
            var shipperName = FormatStaffName(delivery.AssignedStaff);

            string handoverPdfUrl;
            try
            {
                handoverPdfUrl = await _handoverPdfService.GenerateUploadAndResolveUrlAsync(
                    delivery,
                    mealCount,
                    recipientName,
                    signatureBytes,
                    shipperSignatureBytes,
                    proofBytes,
                    proofContentType,
                    shipperName,
                    notes,
                    now,
                    HttpContext.RequestAborted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate handover PDF for delivery {DeliveryId}", id);
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult(
                        "An error occurred while generating handover document",
                        new[] { ex.Message }));
            }

            delivery.ProofImageUrl = proofUrl;
            delivery.ProofCapturedAt = now;
            delivery.DeliveredAt ??= now;
            delivery.DeliveryStatus = "completed";
            delivery.RecipientConfirmedName = recipientName;
            delivery.RecipientSignatureUrl = signatureUrl;
            delivery.ShipperSignatureUrl = shipperSignatureUrl;
            delivery.HandoverDocumentUrl = handoverPdfUrl;
            delivery.RecipientConfirmationCode = null;
            delivery.RecipientConfirmedAt = now;
            if (notes != null)
                delivery.Notes = notes;

            // Keep order in sync (same as status update handler)
            if (delivery.Order != null)
            {
                delivery.Order.Status = OrderLifecycleStatus.Delivered;
                delivery.Order.UpdatedAt = now;
            }

            await _deliveryRepository.UpdateAsync(delivery, HttpContext.RequestAborted);
            await _deliveryRepository.CommitAsync(HttpContext.RequestAborted);

            var refreshed = await _deliveryRepository.GetByIdWithOrderAsync(id, HttpContext.RequestAborted)
                ?? throw new InvalidOperationException("Delivery updated but failed to reload.");

            mealCount = refreshed.Order?.OrderItems?.Sum(i => i.Quantity) ?? 0;
            var resp = new GetShipperDeliveryResponse
            {
                Delivery = new ShipperDeliveryDetailDto
                {
                    DeliveryId = refreshed.Id,
                    OrderId = refreshed.OrderId,
                    DeliveryAddress = refreshed.DeliveryAddress,
                    DeliveryStatus = refreshed.DeliveryStatus,
                    ScheduledDateUtc = refreshed.Order?.ScheduledDate ?? DateTime.MinValue,
                    MealCount = mealCount,
                    DeliveredAtUtc = refreshed.DeliveredAt,
                    ProofImageUrl = refreshed.ProofImageUrl,
                    ProofCapturedAtUtc = refreshed.ProofCapturedAt,
                    Notes = refreshed.Notes,
                    RecipientConfirmedName = refreshed.RecipientConfirmedName,
                    RecipientConfirmedAtUtc = refreshed.RecipientConfirmedAt,
                    RecipientSignatureUrl = refreshed.RecipientSignatureUrl,
                    ShipperSignatureUrl = refreshed.ShipperSignatureUrl,
                    HandoverDocumentUrl = refreshed.HandoverDocumentUrl,
                    RequiresRecipientSignature = false,
                }
            };

            return Ok(BaseApiResponse<GetShipperDeliveryResponse>.SuccessResult(
                resp, "Delivery proof uploaded and handover document generated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading delivery proof {DeliveryId}", id);
            return StatusCode(
                (int)HttpStatusCode.InternalServerError,
                BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("An error occurred while uploading delivery proof", new[] { ex.Message }));
        }
    }

    private int RequireUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var userId))
            throw new UnauthorizedAccessException("Invalid user context.");
        return userId;
    }

    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private async Task<string> UploadDeliveryImageAsync(
        IFormFile file,
        int deliveryId,
        string folder,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var contentType = file.ContentType ?? string.Empty;
        if (!AllowedImageContentTypes.Contains(contentType))
            throw new ArgumentException($"Unsupported image ContentType: {contentType}");

        await using var stream = file.OpenReadStream();
        await using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        return await UploadDeliveryImageBytesAsync(ms.ToArray(), contentType, deliveryId, folder, now, cancellationToken);
    }

    private async Task<string> UploadDeliveryImageBytesAsync(
        byte[] bytes,
        string contentType,
        int deliveryId,
        string folder,
        DateTime now,
        CancellationToken cancellationToken)
    {
        if (!AllowedImageContentTypes.Contains(contentType))
            throw new ArgumentException($"Unsupported image ContentType: {contentType}");

        var ext = contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new ArgumentException($"Unsupported image ContentType: {contentType}")
        };

        var objectName = $"deliveries/{deliveryId:D}/{folder}/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";
        await using (var stream = new MemoryStream(bytes))
        {
            await _storage.UploadObjectAsync(objectName, stream, contentType, cancellationToken);
        }

        var signed = await _storage.CreateSignedUrlAsync(
            objectName,
            System.Net.Http.HttpMethod.Get,
            contentType: null,
            expiresIn: TimeSpan.FromMinutes(30));
        return signed.Url;
    }

    private static string? FormatStaffName(Domain.Entities.User? user)
    {
        if (user == null) return null;
        var name = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(name) ? user.Username : name;
    }
}

public class UploadDeliveryProofForm
{
    public IFormFile File { get; set; } = null!;
    public IFormFile Signature { get; set; } = null!;
    public IFormFile ShipperSignature { get; set; } = null!;
    public string RecipientConfirmedName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

