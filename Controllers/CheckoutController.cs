using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class CheckoutController : Controller
{
    private readonly ICheckoutService _checkoutService;
    private readonly IConfiguration _configuration;
    private readonly ICartService _cartService; // Thêm ICartService để truy cập giỏ hàng


    public CheckoutController(ICheckoutService checkoutService, IConfiguration configuration,ICartService cartService)
    {
        _checkoutService = checkoutService;
        _configuration = configuration;
        _cartService = cartService; // Sử dụng ICartService để truy cập giỏ hàng
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = HttpContext.Session.GetInt32("CurrentUserId");
        if (userId == null)
        {
            TempData["Error"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
            return RedirectToAction("Login", "Login");
        }

        int userIdValue = userId.Value;

        var cartItems = await _cartService.GetCartByUserIdAsync(userIdValue); // Sử dụng ICartService
        var user = await _checkoutService.GetUserByIdAsync(userIdValue); // Giả định có phương thức này
        var paymentMethods = await _checkoutService.GetPaymentMethodsAsync();

        decimal totalAmount = await _cartService.GetTotalAmountAsync(userIdValue); // Sử dụng ICartService

        var viewModel = new CheckoutViewModel
        {
            CartItems = cartItems,
            User = user,
            PaymentMethods = paymentMethods,
            TotalAmount = totalAmount
        };

        return View(viewModel);
    }

    //[HttpPost]
    //public async Task<IActionResult> PlaceOrder(int PaymentMethodId)
    //{
    //    var currentUser = HttpContext.Session.GetObjectFromJson<User>("CurrentUser");
    //    if (currentUser == null || currentUser.UserId == 0)
    //    {
    //        TempData["Error"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
    //        return RedirectToAction("Login", "Login");
    //    }

    //    int userId = currentUser.UserId;
    //    var cartItems = await _checkoutService.GetCartByUserIdAsync(userId);
    //    decimal totalAmount = cartItems.Sum(item => item.Quantity * item.Product.Price);
    //    string orderId = DateTime.Now.Ticks.ToString();

    //    // Tạo và lưu đơn hàng
    //    var order = new Order
    //    {
    //        UserId = userId,
    //        OrderDate = DateTime.Now,
    //        TotalAmount = totalAmount,
    //        Status = "Pending",
    //        PaymentMethodId = PaymentMethodId
    //    };
    //    await _checkoutService.CreateOrderAsync(order);

    //    if (PaymentMethodId == 1) // VNPay
    //    {
    //        var vnpay = new VnpayLibrary(
    //            _configuration["Vnpay:Url"],
    //            _configuration["Vnpay:TmnCode"],
    //            _configuration["Vnpay:HashSecret"]
    //        );

    //        string returnUrl = _configuration["Vnpay:ReturnUrl"];
    //        string paymentUrl = vnpay.GetPaymentUrl(orderId, totalAmount, returnUrl);
    //        return Redirect(paymentUrl);
    //    }

    //    TempData["Message"] = "Phương thức thanh toán chưa được hỗ trợ.";
    //    return RedirectToAction("Checkout");
    //}

    //public IActionResult Return()
    //{
    //    string vnpResponseCode = Request.Query["vnp_ResponseCode"];
    //    string vnpTxnRef = Request.Query["vnp_TxnRef"];
    //    string vnpSecureHash = Request.Query["vnp_SecureHash"];

    //    if (vnpResponseCode == "00") // Thanh toán thành công
    //    {
    //        var order = _checkoutService.GetOrderByIdAsync(vnpTxnRef).Result;
    //        if (order != null)
    //        {
    //            order.Status = "Completed";
    //            _checkoutService.UpdateOrderAsync(order);
    //        }
    //        TempData["Success"] = "Thanh toán thành công!";
    //        return RedirectToAction("PaymentSuccess");
    //    }

    //    TempData["Error"] = $"Thanh toán thất bại. Mã lỗi: {vnpResponseCode}";
    //    return RedirectToAction("PaymentFailed");
    //}

    public IActionResult PaymentSuccess()
    {
        return View();
    }

    public IActionResult PaymentFailed()
    {
        return View();
    }
}