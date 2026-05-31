using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerArea")]
    public class OperationController : Controller
    {
        private readonly BackendMasterDataClient _masterDataClient;
        private readonly BackendComplaintClient _complaintClient;

        public OperationController(
            BackendMasterDataClient masterDataClient,
            BackendComplaintClient complaintClient)
        {
            _masterDataClient = masterDataClient;
            _complaintClient = complaintClient;
        }

        public IActionResult FoodSafety() => View();
        public IActionResult Feedback() => View();
        public IActionResult Complaints() => View();
        public IActionResult ComplaintReview(int id) => View(id);
        public IActionResult Contact() => View();

        [HttpGet]
        public async Task<IActionResult> FeedbackList(
            int page = 1,
            int pageSize = 20,
            string? searchTerm = null,
            int? maxRating = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var data = await _masterDataClient.GetManagerReviewsAsync(token, page, pageSize, searchTerm, maxRating, ct);
                return Json(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyFeedback(int id, [FromBody] ReplyFeedbackRequest request, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var dto = await _masterDataClient.ReplyToReviewAsync(id, request.Reply, token, ct);
                return Json(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ContactList(
            int page = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? status = null,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var data = await _masterDataClient.GetManagerContactInquiriesAsync(
                    token, page, pageSize, searchTerm, status, ct);
                return Json(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyContact(int id, [FromBody] ReplyFeedbackRequest request, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var dto = await _masterDataClient.ReplyToContactInquiryAsync(id, request.Reply, token, ct);
                return Json(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CompanyDocuments(CancellationToken ct)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var data = await _masterDataClient.GetCompanyDocumentsAsync(token, ct);
                return Json(new { documents = data.Documents });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCompanyDocument(
            IFormFile file,
            string title,
            string documentType,
            string? description,
            DateTime? issuedDate,
            DateTime? expiryDate,
            bool isPublished = true,
            int sortOrder = 0,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var doc = await _masterDataClient.UploadCompanyDocumentAsync(
                    file, token, title, documentType, description, issuedDate, expiryDate, isPublished, sortOrder, ct);
                return Json(doc);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCompanyDocument(
            int id,
            [FromBody] UpdateCompanyPublicDocumentClientRequest request,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var doc = await _masterDataClient.UpdateCompanyDocumentAsync(id, request, token, ct);
                return Json(doc);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompanyDocument(int id, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                await _masterDataClient.DeleteCompanyDocumentAsync(id, token, ct);
                return Json(new { ok = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ComplaintList(
            int page = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? status = "pending_review",
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var data = await _complaintClient.GetManagerComplaintsAsync(
                    token, page, pageSize, searchTerm, status, ct);
                return Json(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ComplaintReviewData(int id, CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var dto = await _complaintClient.GetManagerComplaintReviewAsync(id, token, ct);
                return Json(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveComplaint(
            int id,
            [FromBody] ResolveManagerComplaintClientRequest request,
            CancellationToken ct = default)
        {
            var token = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { message = "Not authenticated" });
            try
            {
                var dto = await _complaintClient.ResolveManagerComplaintAsync(id, request, token, ct);
                return Json(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

public sealed class ReplyFeedbackRequest
{
    public string Reply { get; set; } = string.Empty;
}
