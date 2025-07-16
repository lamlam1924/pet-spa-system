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

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(int PaymentMethodId)
    {
        var userId = HttpContext.Session.GetInt32("CurrentUserId");
        if (userId == null)
        {
            TempData["Error"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
            return RedirectToAction("Login", "Login");
        }
        int userIdValue = userId.Value;
        var cartItems = await _cartService.GetCartByUserIdAsync(userIdValue);
        decimal productTotal = await _cartService.GetTotalAmountAsync(userIdValue); // chỉ là tiền hàng
        decimal shippingFee = 30000; // phí giao hàng cố định
        decimal discount = 0; // nếu có mã giảm giá thì trừ ở đây
        decimal totalAmount = productTotal + shippingFee - discount;
        // Tạo đơn hàng
        var order = new Order
        {
            UserId = userIdValue,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
            StatusId = 1, // Pending
            CreatedAt = DateTime.Now,
            ShippingAddress = ""
        };
        await _checkoutService.CreateOrderAsync(order);
        if (PaymentMethodId == 1) // VNPay
        {
            var config = _configuration.GetSection("Vnpay");
            var pay = new VnPayLibrary();
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            pay.AddRequestData("vnp_Version", config["Version"]);
            pay.AddRequestData("vnp_Command", config["Command"]);
            pay.AddRequestData("vnp_TmnCode", config["TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)order.TotalAmount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", config["CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(HttpContext));
            pay.AddRequestData("vnp_Locale", config["Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng {order.OrderId}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", config["PaymentBackReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());
            var paymentUrl = pay.CreateRequestUrl(config["BaseUrl"], config["HashSecret"]);
            return Redirect(paymentUrl);
        }
        TempData["Message"] = "Phương thức thanh toán chưa được hỗ trợ.";
        return RedirectToAction("Checkout");
    }

   [HttpGet]
public async Task<IActionResult> PaymentCallbackVnpay()
{
    var config = _configuration.GetSection("Vnpay");
    var pay = new VnPayLibrary();
    var response = pay.GetFullResponseData(Request.Query, config["HashSecret"]);
    var orderId = response["OrderId"];
    var order = await _checkoutService.GetOrderByIdAsync(orderId);
    if (response["Success"] == "true")
    {
        order.StatusId = 2; // Completed
        await _checkoutService.UpdateOrderAsync(order);

        // XÓA GIỎ HÀNG SAU KHI THANH TOÁN THÀNH CÔNG
        await _cartService.ClearCartAsync(order.UserId);

        TempData["Success"] = "Thanh toán thành công!";
        return RedirectToAction("PaymentSuccess");
    }
    else
    {
        order.StatusId = 3; // Failed
        await _checkoutService.UpdateOrderAsync(order);
        TempData["Error"] = $"Thanh toán thất bại. Mã lỗi: {response["VnPayResponseCode"]}";
        return RedirectToAction("PaymentFailed");
    }
}

    public IActionResult PaymentSuccess()
    {
        return View();
    }

    public IActionResult PaymentFailed()
    {
        return View();
    }
}