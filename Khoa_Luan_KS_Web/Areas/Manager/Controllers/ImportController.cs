using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class ImportController : Controller
    {
        private readonly BackendWarehouseClient _warehouseClient;

        public ImportController(BackendWarehouseClient warehouseClient)
        {
            _warehouseClient = warehouseClient;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            try
            {
                var ingredients = await _warehouseClient.GetIngredientsAsync(token, 1, 500, isActive: true, ct: ct);
                ViewBag.Ingredients = ingredients.Items;
            }
            catch
            {
                ViewBag.Ingredients = new List<IngredientClientDto>();
            }

            return View();
        }

        public IActionResult Create() => RedirectToAction(nameof(Index));
        public IActionResult List() => RedirectToAction(nameof(Index));
        public IActionResult Approve() => RedirectToAction(nameof(Index));
        public IActionResult History() => RedirectToAction(nameof(Index));
        public IActionResult Receipt(string? id) => RedirectToAction(nameof(Index));

        [HttpGet]
        public async Task<IActionResult> Proposals(int page = 1, int pageSize = 100, CancellationToken ct = default)
        {
            var token = RequireToken();
            try
            {
                var data = await _warehouseClient.GetIntakeProposalsAsync(token, page, pageSize, ct);
                return Json(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProposalDetail(int id, CancellationToken ct = default)
        {
            var token = RequireToken();
            try
            {
                var detail = await _warehouseClient.GetIntakeProposalAsync(id, token, ct);
                return Json(detail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProposal([FromBody] CreateIntakeProposalClientRequest request, CancellationToken ct = default)
        {
            var token = RequireToken();
            try
            {
                var created = await _warehouseClient.CreateIntakeProposalAsync(request, token, ct);
                return Json(created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewProposal(int id, [FromBody] ReviewIntakeProposalRequest request, CancellationToken ct = default)
        {
            var token = RequireToken();
            try
            {
                var result = await _warehouseClient.ReviewIntakeProposalAsync(id, request.Approve, request.ReviewNote, token, ct);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReceipt(int id, [FromBody] ConfirmReceiptRequest? request, CancellationToken ct = default)
        {
            var token = RequireToken();
            try
            {
                var payload = new CreateActualReceiptClientRequest
                {
                    ConfirmIngredientsMeetStandard = true,
                    ReceivedAtUtc = DateTime.UtcNow,
                    Note = request?.Note
                };
                var result = await _warehouseClient.CreateActualReceiptAsync(id, payload, token, ct);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string RequireToken()
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Not authenticated");
            return token;
        }
    }

    public sealed class ReviewIntakeProposalRequest
    {
        public bool Approve { get; set; }
        public string? ReviewNote { get; set; }
    }

    public sealed class ConfirmReceiptRequest
    {
        public string? Note { get; set; }
    }
}
