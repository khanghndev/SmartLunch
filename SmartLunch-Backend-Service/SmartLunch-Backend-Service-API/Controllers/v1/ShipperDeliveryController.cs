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

    public ShipperDeliveryController(
        ILogger<ShipperDeliveryController> logger,
        IMediator mediator,
        IStorageService storage,
        IDeliveryRepository deliveryRepository)
    {
        _logger = logger;
        _mediator = mediator;
        _storage = storage;
        _deliveryRepository = deliveryRepository;
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
    /// Xác nhận giao hàng: ảnh PoD + chữ ký người nhận + tên người nhận.
    /// </summary>
    [HttpPost("{id:int}/proof")]
    [Authorize(Policy = "permission:deliveries.update")]
    [RequestSizeLimit(15 * 1024 * 1024)]
    public async Task<ActionResult<BaseApiResponse<GetShipperDeliveryResponse>>> UploadProof(
        int id,
        [FromForm] IFormFile file,
        [FromForm] IFormFile signature,
        [FromForm] UploadDeliveryProofRequest request)
    {
        try
        {
            var shipperId = RequireUserId();
            if (file == null || file.Length <= 0)
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("File is required", new[] { "Missing proof photo." }));

            if (signature == null || signature.Length <= 0)
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult("Signature is required", new[] { "Missing recipient signature." }));

            if (string.IsNullOrWhiteSpace(request?.RecipientConfirmedName))
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
            string proofUrl;
            string signatureUrl;
            try
            {
                proofUrl = await UploadDeliveryImageAsync(file, id, "proof", now, HttpContext.RequestAborted);
                signatureUrl = await UploadDeliveryImageAsync(signature, id, "signature", now, HttpContext.RequestAborted);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(BaseApiResponse<GetShipperDeliveryResponse>.ErrorResult(ex.Message, new[] { ex.Message }));
            }

            delivery.ProofImageUrl = proofUrl;
            delivery.ProofCapturedAt = now;
            delivery.DeliveredAt ??= now;
            delivery.DeliveryStatus = "completed";
            delivery.RecipientConfirmedName = request!.RecipientConfirmedName.Trim();
            delivery.RecipientSignatureUrl = signatureUrl;
            delivery.RecipientConfirmationCode = null;
            delivery.RecipientConfirmedAt = now;
            if (!string.IsNullOrWhiteSpace(request.Notes))
                delivery.Notes = request.Notes.Trim();

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

            var mealCount = refreshed.Order?.OrderItems?.Sum(i => i.Quantity) ?? 0;
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
                    RequiresRecipientSignature = false,
                }
            };

            return Ok(BaseApiResponse<GetShipperDeliveryResponse>.SuccessResult(resp, "Delivery proof uploaded successfully"));
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

        var ext = contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => throw new ArgumentException($"Unsupported image ContentType: {contentType}")
        };

        var objectName = $"deliveries/{deliveryId:D}/{folder}/{now:yyyy}/{now:MM}/{Guid.NewGuid():N}{ext}";
        await using (var stream = file.OpenReadStream())
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
}

