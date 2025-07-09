using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;

public class CheckoutController : Controller
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
        //    _vnpayService = vnpayService;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        // Lấy người dùng hiện tại từ session
        var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
        if (currentUser == null || currentUser.UserId == 0)
        {
            TempData["Error"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
            return RedirectToAction("Login", "Login");
        }

        int userId = currentUser.UserId;

        // Lấy dữ liệu từ service
        var cartItems = await _checkoutService.GetCartByUserIdAsync(userId);
        var user = await _checkoutService.GetUserByIdAsync(userId);
        var paymentMethods = await _checkoutService.GetPaymentMethodsAsync();

        // Tính tổng
        decimal total = cartItems.Sum(c => c.Quantity * c.Product.Price);

        // Tạo view model
        var viewModel = new CheckoutViewModel
        {
            CartItems = cartItems,
            User = user,
            PaymentMethods = paymentMethods,
            TotalAmount = total
        };

        return View(viewModel);
    }

    //[HttpPost]
    //public IActionResult PaymentWithVNPay(decimal total)
    //{
    //    var tick = DateTime.UtcNow.Ticks.ToString();
    //    var vnpay = new VnPayLibrary();

    //    vnpay.AddRequestData("vnp_Version", VNPayConfig.vnp_Version);
    //    vnpay.AddRequestData("vnp_Command", VNPayConfig.vnp_Command);
    //    vnpay.AddRequestData("vnp_TmnCode", VNPayConfig.vnp_TmnCode);
    //    vnpay.AddRequestData("vnp_Amount", ((int)total * 100).ToString()); // nhân 100 vì VNPay dùng đơn vị là VND x 100
    //    vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
    //    vnpay.AddRequestData("vnp_CurrCode", "VND");
    //    vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress.ToString());
    //    vnpay.AddRequestData("vnp_Locale", "vn");
    //    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
    //    vnpay.AddRequestData("vnp_OrderType", "billpayment");
    //    vnpay.AddRequestData("vnp_ReturnUrl", VNPayConfig.vnp_Returnurl);
    //    vnpay.AddRequestData("vnp_TxnRef", tick); // mã giao dịch

    //    string paymentUrl = vnpay.CreateRequestUrl(VNPayConfig.vnp_Url, VNPayConfig.vnp_HashSecret);
    //    return Redirect(paymentUrl);
    //}
    //public IActionResult VnPayReturn()
    //{
    //    var vnpayData = Request.Query;
    //    var vnpay = new VnPayLibrary();
    //    foreach (var (key, value) in vnpayData)
    //    {
    //        if (key.StartsWith("vnp_"))
    //            vnpay.AddResponseData(key, value);
    //    }

    //    string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
    //    bool isValid = vnpay.ValidateSignature(VNPayConfig.vnp_HashSecret);

    //    if (isValid && responseCode == "00")
    //    {
    //        ViewBag.Message = "Thanh toán thành công!";
    //        // TODO: lưu đơn hàng, cập nhật trạng thái đơn hàng, gửi mail...
    //    }
    //    else
    //    {
    //        ViewBag.Message = "Thanh toán thất bại!";
    //    }

    //    return View();
    //}


    //[HttpPost]
    //public async Task<IActionResult> PlaceOrder(int PaymentMethodId)
    //{
    //    var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
    //    if (currentUser == null)
    //    {
    //        return RedirectToAction("Login", "Login");
    //    }

    //    // Nếu là VNPay
    //    if (PaymentMethodId == 1) // ID 2 là VNPay
    //    {
    //        // Tạo URL redirect sang trang thanh toán VNPay
    //        string paymentUrl = _vnpayService.CreatePaymentUrl(currentUser.UserId, HttpContext);

    //        return Redirect(paymentUrl);
    //    }

    //    // Xử lý các phương thức khác (COD...)
    //    // Lưu đơn hàng, chuyển đến trang xác nhận...
    //    return View("OrderConfirmation");
    //}

}
