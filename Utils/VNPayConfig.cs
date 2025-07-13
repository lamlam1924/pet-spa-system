namespace pet_spa_system1.Utils
{
    public class VNPayConfig
    {
        public const string vnp_Version = "2.1.0";
        public const string vnp_Command = "pay";
        public const string vnp_TmnCode = "7YPT5VT1";
        public const string vnp_HashSecret = "FM1VYWBQL0CGFLMC93RAGT6YO3PZKG6N";
        public const string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public const string vnp_Returnurl = "https://localhost:7231/Checkout/VnPayReturn"; // thay bằng domain thật
    }
}
