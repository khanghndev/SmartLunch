using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Khoa_Luan_KS_Web.Helpers;
using Khoa_Luan_KS_Web.Models;
using Khoa_Luan_KS_Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khoa_Luan_KS_Web.Controllers
{
    [Authorize(Policy = "CustomerArea")]
    public class ProfileController : Controller
    {
        private readonly Services.BackendAuthClient _backendAuthClient;
        private readonly Services.BackendMasterDataClient _masterDataClient;
        private readonly Services.BackendCompanyProfileClient _companyProfileClient;
        private readonly Services.IApiTokenService _apiTokenService;

        public ProfileController(
            Services.BackendAuthClient backendAuthClient,
            Services.BackendMasterDataClient masterDataClient,
            Services.BackendCompanyProfileClient companyProfileClient,
            Services.IApiTokenService apiTokenService)
        {
            _backendAuthClient = backendAuthClient;
            _masterDataClient = masterDataClient;
            _companyProfileClient = companyProfileClient;
            _apiTokenService = apiTokenService;
        }

        private static bool IsOrganizationAccount(ClaimsPrincipal user) =>
            user.IsInRole("Organization") ||
            user.IsInRole("Company") ||
            user.IsInRole("Khách hàng doanh nghiệp");

        public async Task<IActionResult> Security(CancellationToken ct)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Security)) });

            var vm = new ChangePasswordViewModel
            {
                SuccessMessage = TempData["PasswordSuccess"] as string,
                ErrorMessage = TempData["PasswordError"] as string,
            };

            try
            {
                var profile = await _backendAuthClient.GetProfileAsync(accessToken, ct);
                vm.Email = profile.Email;
                vm.FullName = profile.FullName;
            }
            catch (Exception ex)
            {
                vm.ErrorMessage = ex.Message;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken ct)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Security)) });

            if (!ModelState.IsValid)
            {
                try
                {
                    var profile = await _backendAuthClient.GetProfileAsync(accessToken, ct);
                    model.Email = profile.Email;
                    model.FullName = profile.FullName;
                }
                catch { /* ignore */ }
                return View("Security", model);
            }

            try
            {
                await _backendAuthClient.ChangePasswordAsync(
                    new ChangePasswordClientRequest
                    {
                        CurrentPassword = model.CurrentPassword,
                        NewPassword = model.NewPassword,
                        ConfirmPassword = model.ConfirmPassword,
                    },
                    accessToken,
                    ct);

                _apiTokenService.ClearTokens();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Success"] = "Đã đổi mật khẩu thành công. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                TempData["PasswordError"] = ex.Message;
                return RedirectToAction(nameof(Security));
            }
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index)) });

            var vm = new CompanyProfilePageViewModel
            {
                IsOrganizationAccount = IsOrganizationAccount(User),
                SuccessMessage = TempData["ProfileSuccess"] as string,
                ErrorMessage = TempData["ProfileError"] as string,
            };

            try
            {
                vm.User = await _backendAuthClient.GetProfileAsync(accessToken, ct);
                if (vm.IsOrganizationAccount)
                    vm.Organization = await _companyProfileClient.GetProfileAsync(accessToken, ct);
            }
            catch (Exception ex)
            {
                vm.LoadError = ex.Message;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrganization(
            UpdateOrganizationProfileClientRequest request,
            CancellationToken ct)
        {
            if (!IsOrganizationAccount(User))
                return Forbid();

            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index)) });

            try
            {
                await _companyProfileClient.UpdateProfileAsync(request, accessToken, ct);
                TempData["ProfileSuccess"] = "Đã lưu hồ sơ doanh nghiệp thành công.";
            }
            catch (Exception ex)
            {
                TempData["ProfileError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLogo(IFormFile logoFile, CancellationToken ct)
        {
            if (!IsOrganizationAccount(User))
                return Forbid();

            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index)) });

            try
            {
                await _companyProfileClient.UploadLogoAsync(logoFile, accessToken, ct);
                TempData["ProfileSuccess"] = "Đã cập nhật ảnh đại diện.";
            }
            catch (Exception ex)
            {
                TempData["ProfileError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadLegalDocument(
            IFormFile file,
            string title,
            string documentType,
            string? description,
            DateTime? issuedDate,
            DateTime? expiryDate,
            CancellationToken ct)
        {
            if (!IsOrganizationAccount(User))
                return Forbid();

            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index)) });

            try
            {
                await _companyProfileClient.UploadDocumentAsync(
                    file, accessToken, title, documentType, description, issuedDate, expiryDate, ct);
                TempData["ProfileSuccess"] = "Đã tải tài liệu pháp lý lên hệ thống.";
            }
            catch (Exception ex)
            {
                TempData["ProfileError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLegalDocument(int documentId, CancellationToken ct)
        {
            if (!IsOrganizationAccount(User))
                return Forbid();

            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Index)) });

            try
            {
                await _companyProfileClient.DeleteDocumentAsync(documentId, accessToken, ct);
                TempData["ProfileSuccess"] = "Đã xóa tài liệu.";
            }
            catch (Exception ex)
            {
                TempData["ProfileError"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Orders(int page = 1, CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Orders)) });

            try
            {
                var orders = await _masterDataClient.GetOrdersAsync(accessToken, page, 20, ct: ct);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tải danh sách đơn: " + ex.Message;
                return View(new Services.GetOrdersClientResponse());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId, int page = 1, CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Orders)) });

            try
            {
                await _masterDataClient.CancelCustomerOrderAsync(orderId, accessToken, ct);
                TempData["OrderSuccess"] = "Đã hủy đơn hàng thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Orders), new { page });
        }

        public async Task<IActionResult> OrderDetail(int id, CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(OrderDetail), new { id }) });

            try
            {
                var res = await _masterDataClient.GetOrderAsync(id, accessToken, ct);
                ViewBag.SignatureDataUrl = HttpContext.Session.GetString($"order_sig_{id}");
                var deliveryJson = HttpContext.Session.GetString($"order_delivery_{id}");
                if (!string.IsNullOrEmpty(deliveryJson))
                {
                    try
                    {
                        var notes = JsonSerializer.Deserialize<List<OrderDeliveryNoteLine>>(deliveryJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        ViewBag.OrderDeliveryNotes = notes;
                    }
                    catch
                    {
                        /* ignore */
                    }
                }

                return View(res.Order);
            }
            catch (Exception)
            {
                TempData["Error"] = "Không thể tải đơn hàng hoặc bạn không có quyền xem.";
                return RedirectToAction(nameof(Orders));
            }
        }

        public async Task<IActionResult> Invoice(int id, CancellationToken ct = default)
        {
            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Invoice), new { id }) });

            try
            {
                var res = await _masterDataClient.GetOrderAsync(id, accessToken, ct);
                var order = res.Order;

                var signedPdfUrl = OrderSignedDocumentHelper.GetSignedPdfUrl(order);
                if (!string.IsNullOrEmpty(signedPdfUrl))
                    return Redirect(signedPdfUrl);

                if (IsOrganizationAccount(User))
                {
                    TempData["Error"] =
                        "Đơn này chưa có file PDF hợp đồng/phụ lục đã ký. Vui lòng ký phụ lục trước khi tải.";
                    return RedirectToAction(nameof(Contracts), new { orderId = id });
                }

                ViewBag.SignatureDataUrl = HttpContext.Session.GetString($"order_sig_{id}");
                return View(order);
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Contracts(CancellationToken ct = default)
        {
            var vm = new CustomerContractsPageVm();
            if (!IsOrganizationAccount(User))
            {
                vm.InfoMessage =
                    "Cổng ký hợp đồng điện tử dành cho tài khoản doanh nghiệp (đơn vị) đã được gán trên hệ thống. Khách cá nhân có thể xem hóa đơn và chữ ký xác nhận trên từng đơn hàng.";
                return View(vm);
            }

            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Contracts)) });

            try
            {
                var res = await _masterDataClient.GetMyOrganizationContractsAsync(accessToken, ct);
                vm.Contracts = res.Contracts ?? new List<CustomerContractDto>();
            }
            catch (Exception ex)
            {
                vm.ApiError = ex.Message;
            }

            vm.PostCheckoutBanner = TempData["OrderSuccess"] as string;
            var placedStr = TempData["OrderPlacedId"] as string;
            var checkoutOrderId = int.TryParse(placedStr, out var chk) ? chk : (int?)null;

            var pendingAnnexOrderId = int.TryParse(Request.Query["orderId"], out var qOid) ? qOid : checkoutOrderId;
            vm.PostCheckoutOrderId = checkoutOrderId ?? pendingAnnexOrderId;

            if (pendingAnnexOrderId is int pid && pid > 0)
            {
                try
                {
                    var ord = await _masterDataClient.GetOrderAsync(pid, accessToken, ct);
                    vm.PendingOrderAnnexJson = SerializeOrderAnnexPortal(ord.Order);
                }
                catch
                {
                    /* ignore — user may not own order */
                }
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OrderAnnexPreview(int orderId, CancellationToken ct = default)
        {
            if (!IsOrganizationAccount(User))
                return Forbid();

            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized();

            try
            {
                var pdf = await _masterDataClient.GetOrderAnnexPreviewPdfAsync(orderId, accessToken, ct);
                Response.Headers.CacheControl = "no-store";
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tạo được bản xem trước PDF: {ex.Message}");
            }
        }

        private static string? SerializeOrderAnnexPortal(OrderDetailClientDto o)
        {
            if (o.Id == 0 || o.AnnexSignedAt.HasValue)
                return null;

            var portal = new
            {
                id = o.Id,
                invoiceCode = o.InvoiceCode,
                orderDate = o.OrderDate.ToString("o"),
                scheduledDate = o.ScheduledDate.ToString("o"),
                totalAmount = o.TotalAmount,
                annexSignedAt = o.AnnexSignedAt,
                contractId = o.ContractSummary?.Id,
                organizationName = o.OrganizationName,
                partnerLegalName = "Công ty Cổ phần HuitMeal",
                items = o.Items.Select(i => new
                {
                    dishName = i.DishName,
                    quantity = i.Quantity,
                    unitPrice = i.UnitPrice,
                    totalPrice = i.TotalPrice,
                }).ToList(),
            };

            return JsonSerializer.Serialize(portal, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignContract(int contractId, string digitalSignature, CancellationToken ct = default)
        {
            if (!IsOrganizationAccount(User))
                return Forbid();

            var sig = (digitalSignature ?? string.Empty).Trim();
            if (contractId <= 0 || string.IsNullOrWhiteSpace(sig))
            {
                TempData["ContractError"] = "Thiếu dữ liệu chữ ký.";
                return RedirectToAction(nameof(Contracts));
            }

            const int maxSig = 400_000;
            if (sig.Length > maxSig)
                sig = sig.Substring(0, maxSig);

            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Contracts)) });

            try
            {
                await _masterDataClient.SignCompanyContractAsync(
                    contractId,
                    new SignCompanyContractRequest { DigitalSignature = sig },
                    accessToken,
                    ct);
                TempData["ContractSuccess"] = "Đã ký số thành công. PDF hợp đồng đã được tạo và lưu trên hệ thống lưu trữ.";
            }
            catch (Exception ex)
            {
                TempData["ContractError"] = ex.Message;
            }

            return RedirectToAction(nameof(Contracts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOrderAnnex(int orderId, string digitalSignature, CancellationToken ct = default)
        {
            if (!IsOrganizationAccount(User))
                return Forbid();

            var sig = (digitalSignature ?? string.Empty).Trim();
            if (orderId <= 0 || string.IsNullOrWhiteSpace(sig))
            {
                TempData["ContractError"] = "Thiếu dữ liệu chữ ký phụ lục đơn.";
                return RedirectToAction(nameof(Contracts));
            }

            const int maxSig = 400_000;
            if (sig.Length > maxSig)
                sig = sig.Substring(0, maxSig);

            var accessToken = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(accessToken))
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action(nameof(Contracts)) });

            try
            {
                await _masterDataClient.SignOrderAnnexAsync(
                    orderId,
                    new SignOrderAnnexApiRequest { DigitalSignature = sig },
                    accessToken,
                    ct);
                TempData["OrderSuccess"] =
                    "Đã ký và lưu PDF phụ lục lên cloud. Trạng thái đơn: đang chờ thanh toán — vui lòng thanh toán đặt cọc từ lịch sử đơn hàng.";
                return RedirectToAction(nameof(Orders));
            }
            catch (Exception ex)
            {
                TempData["ContractError"] = "Không lưu được phụ lục: " + ex.Message;
                return RedirectToAction(nameof(Contracts), new { orderId });
            }
        }
    }
}
