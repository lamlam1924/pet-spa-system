//using pet_spa_system1.Utils;

//namespace pet_spa_system1.Services
//{
//    public class VNPayService
//    {
//        public string CreatePaymentUrl(int userId, HttpContext httpContext)
//        {
//            var tick = DateTime.Now.Ticks.ToString();

//            var vnpay = new VnPayLibrary();
//            vnpay.AddRequestData("vnp_Version", "2.1.0");
//            vnpay.AddRequestData("vnp_Command", "pay");
//            vnpay.AddRequestData("vnp_TmnCode", VNPayConfig.vnp_TmnCode);
//            vnpay.AddRequestData("vnp_Amount", (100000 * 100).ToString()); // Tổng tiền x100
//            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
//            vnpay.AddRequestData("vnp_CurrCode", "VND");
//            vnpay.AddRequestData("vnp_IpAddr", httpContext.Connection.RemoteIpAddress.ToString());
//            vnpay.AddRequestData("vnp_Locale", "vn");
//            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {tick}");
//            vnpay.AddRequestData("vnp_OrderType", "billpayment");
//            vnpay.AddRequestData("vnp_ReturnUrl", VNPayConfig.vnp_ReturnUrl);
//            vnpay.AddRequestData("vnp_TxnRef", tick);

//            string paymentUrl = vnpay.CreateRequestUrl(VNPayConfig.vnp_Url, VNPayConfig.vnp_HashSecret);
//            return paymentUrl;
//        }
//    }
//}
