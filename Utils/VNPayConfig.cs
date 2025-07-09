namespace pet_spa_system1.Utils
{
    public class VNPayConfig
    {
        public const string vnp_Version = "2.1.0";
        public const string vnp_Command = "pay";
        public const string vnp_TmnCode = "YOUR_TMNCODE";
        public const string vnp_HashSecret = "YOUR_SECRET_KEY";
        public const string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public const string vnp_Returnurl = "https://localhost:7231/Checkout/VnPayReturn"; // thay bằng domain thật
    }
}
