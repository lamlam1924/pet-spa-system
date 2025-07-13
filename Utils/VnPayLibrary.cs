using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

public class VnpayLibrary
{
    public const string Version = "2.1.0";
    private readonly string _vnpUrl;
    private readonly string _vnpTmnCode;
    private readonly string _vnpHashSecret;
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnpayCompare());

    public VnpayLibrary(string vnpUrl, string vnpTmnCode, string vnpHashSecret)
    {
        _vnpUrl = vnpUrl;
        _vnpTmnCode = vnpTmnCode;
        _vnpHashSecret = vnpHashSecret;
    }

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _requestData[key] = value;
        }
    }

    public string GetPaymentUrl(string orderId, decimal amount, string returnUrl)
    {
        _requestData["vnp_Version"] = Version;
        _requestData["vnp_Command"] = "pay";
        _requestData["vnp_TmnCode"] = _vnpTmnCode;
        _requestData["vnp_Amount"] = ((int)(amount * 100)).ToString(); // Số tiền * 100 (VND)
        _requestData["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss");
        _requestData["vnp_CurrCode"] = "VND";
        _requestData["vnp_IpAddr"] = "127.0.0.1"; // Thay bằng IP thực tế nếu cần
        _requestData["vnp_Locale"] = "vn";
        _requestData["vnp_OrderInfo"] = $"Thanh toan don hang {orderId}";
        _requestData["vnp_OrderType"] = "other";
        _requestData["vnp_ReturnUrl"] = returnUrl;
        _requestData["vnp_TxnRef"] = orderId;

        // Tạo chữ ký bảo mật
        var data = string.Join("&", _requestData.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));
        var signData = data + _vnpHashSecret;
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(signData));
            var secureHash = BitConverter.ToString(hash).Replace("-", "").ToLower();
            _requestData["vnp_SecureHash"] = secureHash;
        }

        var url = new StringBuilder(_vnpUrl + "?");
        foreach (var kv in _requestData)
        {
            url.Append($"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}&");
        }

        return url.ToString().TrimEnd('&');
    }
}

public class VnpayCompare : IComparer<string>
{
    public int Compare(string x, string y)
    {
        return string.Compare(x, y);
    }
}