using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Models;
using pet_spa_system1.Services;
using pet_spa_system1.Utils;
using pet_spa_system1.ViewModel;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class CheckoutController : Controller
{
    private readonly ICheckoutService _checkoutService;
    private readonly IConfiguration _configuration;
    private readonly ICartService _cartService; // Thêm ICartService để truy cập giỏ hàng
private readonly IEmailService _emailService;
private readonly IPaymentService _paymentService;

public CheckoutController(ICheckoutService checkoutService, IConfiguration configuration, ICartService cartService, IEmailService emailService, IPaymentService paymentService)
{
    _checkoutService = checkoutService;
    _configuration = configuration;
    _cartService = cartService;
    _emailService = emailService;
    _paymentService = paymentService;
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
public async Task<IActionResult> PlaceOrder(int PaymentMethodId, string FullName, string Email, string Phone, string Address)
{
    var userId = HttpContext.Session.GetInt32("CurrentUserId");
    if (userId == null)
    {
        TempData["Error"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
        return RedirectToAction("Login", "Login");
    }
    int userIdValue = userId.Value;

    // Cập nhật lại thông tin user nếu cần
    var user = await _checkoutService.GetUserByIdAsync(userIdValue);
    bool needUpdate = false;
    if (user.FullName != FullName) { user.FullName = FullName; needUpdate = true; }
    
    if (user.Phone != Phone) { user.Phone = Phone; needUpdate = true; }
    if (user.Address != Address) { user.Address = Address; needUpdate = true; }
    if (needUpdate)
    {
        await _checkoutService.UpdateUserAsync(user); // Bạn cần có hàm này trong service/repo
    }

    var cartItems = await _cartService.GetCartByUserIdAsync(userIdValue);
    decimal productTotal = await _cartService.GetTotalAmountAsync(userIdValue);
    decimal shippingFee = 30000;
    decimal discount = 0;
    decimal totalAmount = productTotal + shippingFee - discount;
    if (string.IsNullOrEmpty(Address))
{
    TempData["Error"] = "Địa chỉ nhận hàng không được để trống.";
    return RedirectToAction("Checkout");
}
    // Tạo đơn hàng, truyền ShippingAddress lấy từ Address vừa nhập
    var order = new Order
    {
        UserId = userIdValue,
        OrderDate = DateTime.Now,
        TotalAmount = totalAmount,
        StatusId = 1, // Pending
        CreatedAt = DateTime.Now,
        ShippingAddress = Address, // <-- Lưu địa chỉ nhận hàng
    };
    await _checkoutService.CreateOrderAsync(order);
        // Tạo các OrderItem từ giỏ hàng
var orderItems = cartItems.Select(cart => new OrderItem
{
    OrderId = order.OrderId,
    ProductId = cart.ProductId,
    Quantity = cart.Quantity,
    UnitPrice = cart.Product.Price
}).ToList();

// Lưu các OrderItem vào DB
await _checkoutService.AddOrderItemsAsync(orderItems);
        
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

        // Lấy thông tin user
        var user = order.User;
        // Lấy danh sách sản phẩm vừa đặt
        var items = order.OrderItems.Select(oi => new OrderItemDetail
        {
            ProductName = oi.Product.Name,
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice,
            ImageUrl = oi.Product.ImageUrl
        }).ToList();

            // Tạo ViewModel xác nhận đơn hàng
            var orderVm = new OrderConfirmationViewModel
            {
                OrderId = order.OrderId,
                CustomerName = user.FullName ?? user.Username,
                Email = user.Email,

                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Items = items
            };

        // Gửi email xác nhận
        _emailService.SendOrderConfirmation(orderVm);

        // Lưu thông tin payment
        string transactionNo = "";
        response.TryGetValue("vnp_TransactionNo", out transactionNo);
        if (string.IsNullOrEmpty(transactionNo))
            response.TryGetValue("vnp_TransactionNo", out transactionNo);

        var payment = new Payment
{
    OrderId = order.OrderId,
    UserId = order.UserId, // <-- Bổ sung dòng này
    Amount = order.TotalAmount,
    PaymentMethodId = 1,
    TransactionId = transactionNo,
    PaymentDate = DateTime.Now
};
        _paymentService.AddPayment(payment);

        // XÓA GIỎ HÀNG SAU KHI THANH TOÁN THÀNH CÔNG
        await _cartService.ClearCartAsync(order.UserId);

        // Truyền ViewModel sang View
        return View("PaymentSuccess", orderVm);
    }
    else
    {
        order.StatusId = 3; // Failed
        await _checkoutService.UpdateOrderAsync(order);
        TempData["Error"] = $"Thanh toán thất bại. Mã lỗi: {response["VnPayResponseCode"]}";
        return RedirectToAction("PaymentFailed");
    }
}

   public IActionResult PaymentSuccess(OrderConfirmationViewModel model)
{
    return View(model);
}

    public IActionResult PaymentFailed()
    {
        return View();
    }
}