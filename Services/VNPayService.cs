using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using pet_spa_system1.Models;
using System.Collections.Generic;

namespace pet_spa_system1.Services
{
    public class VNPayService
    {
        private readonly IConfiguration _configuration;
        public VNPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(Order order, HttpContext context)
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
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", config["Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng {order.OrderId}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", config["PaymentBackReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());
            var paymentUrl = pay.CreateRequestUrl(config["BaseUrl"], config["HashSecret"]);
            return paymentUrl;
        }

        public Dictionary<string, string> PaymentExecute(IQueryCollection collections)
        {
            var config = _configuration.GetSection("Vnpay");
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, config["HashSecret"]);
            return response;
        }
    }
}
