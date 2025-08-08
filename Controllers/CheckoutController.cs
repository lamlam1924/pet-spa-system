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
    private readonly IProductService _productService;

public CheckoutController(ICheckoutService checkoutService, IConfiguration configuration, ICartService cartService, IEmailService emailService, IPaymentService paymentService, IProductService productService)
    {
        _checkoutService = checkoutService;
        _configuration = configuration;
        _cartService = cartService;
        _emailService = emailService;
        _paymentService = paymentService;
        _productService = productService;
       
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

        if (string.IsNullOrEmpty(Address))
        {
            TempData["Error"] = "Địa chỉ nhận hàng không được để trống.";
            return RedirectToAction("Checkout");
        }

        // Cập nhật lại thông tin user nếu cần
        var user = await _checkoutService.GetUserByIdAsync(userIdValue);
        bool needUpdate = false;
        if (user.FullName != FullName) { user.FullName = FullName; needUpdate = true; }
        if (user.Phone != Phone) { user.Phone = Phone; needUpdate = true; }
        if (user.Address != Address) { user.Address = Address; needUpdate = true; }
        if (needUpdate)
        {
            await _checkoutService.UpdateUserAsync(user);
        }

        var cartItems = await _cartService.GetCartByUserIdAsync(userIdValue);
        var productTotal = await _cartService.GetTotalAmountAsync(userIdValue);
        decimal shippingFee = 30000;
        decimal discount = 0;
        decimal totalAmount = productTotal + shippingFee - discount;

        if (PaymentMethodId == 1) // VNPay
        {
            var config = _configuration.GetSection("Vnpay");
            var pay = new VnPayLibrary();
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

            string tempOrderId = Guid.NewGuid().ToString("N");

            pay.AddRequestData("vnp_Version", config["Version"]);
            pay.AddRequestData("vnp_Command", config["Command"]);
            pay.AddRequestData("vnp_TmnCode", config["TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)totalAmount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", config["CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(HttpContext));
            pay.AddRequestData("vnp_Locale", config["Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng {tempOrderId}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", config["PaymentBackReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", tempOrderId);

            // Lưu tạm vào session
            HttpContext.Session.SetString("TempOrderId", tempOrderId);
            HttpContext.Session.SetInt32("TempUserId", userIdValue);
            HttpContext.Session.SetString("TempAddress", Address);

            var paymentUrl = pay.CreateRequestUrl(config["BaseUrl"], config["HashSecret"]);
            return Redirect(paymentUrl);
        }

        // Thanh toán COD
        if (PaymentMethodId == 3)
        {
            // Tạo đơn hàng
            var order = new Order
            {
                UserId = userIdValue,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                StatusId = 1, // Pending
                CreatedAt = DateTime.Now,
                ShippingAddress = Address
            };
            await _checkoutService.CreateOrderAsync(order);

            var orderItems = cartItems.Select(cart => new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = cart.ProductId,
                Quantity = cart.Quantity,
                UnitPrice = cart.Product.Price
            }).ToList();
            await _checkoutService.AddOrderItemsAsync(orderItems);
            foreach (var item in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    await _productService.UpdateProductAsync(product);
                }
            }

            var items = orderItems.Select(oi => new OrderItemDetail
            {
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                ImageUrl = oi.Product.ImageUrl
            }).ToList();

            var orderVm = new OrderConfirmationViewModel
            {
                OrderId = order.OrderId,
                CustomerName = user.FullName ?? user.Username,
                Email = user.Email,
                ShippingAddress = order.ShippingAddress,
                TotalAmount = order.TotalAmount,
                Items = items
            };

            _emailService.SendOrderConfirmation(orderVm);

            var payment = new Payment
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                Amount = order.TotalAmount,
                PaymentMethodId = 3,
                TransactionId = null,
                PaymentDate = DateTime.Now
            };
            _paymentService.AddPayment(payment);

            await _cartService.ClearCartAsync(order.UserId);

            return View("PaymentSuccess", orderVm);
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
        var tempOrderId = response["OrderId"]; // GUID tạm

        var success = response["Success"] == "true";

        if (!success)
        {
            TempData["Error"] = $"Thanh toán thất bại. Mã lỗi: {response["VnPayResponseCode"]}";
            return RedirectToAction("PaymentFailed");
        }

        // Lấy dữ liệu từ session
        var userId = HttpContext.Session.GetInt32("TempUserId") ?? 0;
        var address = HttpContext.Session.GetString("TempAddress");

        var user = await _checkoutService.GetUserByIdAsync(userId);
        var cartItems = await _cartService.GetCartByUserIdAsync(userId);
        var productTotal = await _cartService.GetTotalAmountAsync(userId);
        decimal shippingFee = 30000;
        decimal discount = 0;
        decimal totalAmount = productTotal + shippingFee - discount;

        // Tạo đơn hàng thực sự
        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            TotalAmount = totalAmount,
            StatusId = 2, // Completed
            CreatedAt = DateTime.Now,
            ShippingAddress = address
        };
        await _checkoutService.CreateOrderAsync(order);

        var orderItems = cartItems.Select(cart => new OrderItem
        {
            OrderId = order.OrderId,
            ProductId = cart.ProductId,
            Quantity = cart.Quantity,
            UnitPrice = cart.Product.Price
        }).ToList();
        await _checkoutService.AddOrderItemsAsync(orderItems);

        // Lưu payment
        response.TryGetValue("vnp_TransactionNo", out string transactionNo);
        var payment = new Payment
        {
            OrderId = order.OrderId,
            UserId = userId,
            Amount = totalAmount,
            PaymentMethodId = 1,
            PaymentStatusId = 2,
            TransactionId = transactionNo,
            PaymentDate = DateTime.Now
        };
        _paymentService.AddPayment(payment);

        // Gửi email
        var items = orderItems.Select(oi => new OrderItemDetail
        {
            ProductName = oi.Product?.Name ?? "Sản phẩm",
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice,
            ImageUrl = oi.Product?.ImageUrl
        }).ToList();

        var orderVm = new OrderConfirmationViewModel
        {
            OrderId = order.OrderId,
            CustomerName = user.FullName ?? user.Username,
            Email = user.Email,
            ShippingAddress = order.ShippingAddress,
            TotalAmount = order.TotalAmount,
            Items = items
        };
        _emailService.SendOrderConfirmation(orderVm);

        // Trừ kho
        foreach (var item in orderItems)
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            product.Stock -= item.Quantity;
            await _productService.UpdateProductAsync(product);
        }

        await _cartService.ClearCartAsync(userId);
        return View("PaymentSuccess", orderVm);
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