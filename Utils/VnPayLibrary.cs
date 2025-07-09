using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace pet_spa_system1.Utils
{ 
    public class VnPayLibrary
    {
        public const string VERSION = "2.1.0";
        public const string PAY_COMMAND = "pay";
        public const string QUERYDR_COMMAND = "querydr";
        public const string REFUND_COMMAND = "refund";

        private readonly SortedList<string, string> requestData = new SortedList<string, string>();
        private readonly SortedList<string, string> responseData = new SortedList<string, string>();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                requestData[key] = value;
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                responseData[key] = value;
            }
        }

        public string GetResponseData(string key)
        {
            return responseData.ContainsKey(key) ? responseData[key] : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var queryString = new StringBuilder();
            foreach (var kv in requestData)
            {
                queryString.Append(WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes($"{kv.Key}={kv.Value}")) + "&");
            }

            string rawData = string.Join("&", requestData.Select(kv => $"{kv.Key}={kv.Value}"));
            string secureHash = HmacSHA512(hashSecret, rawData);
            string fullUrl = baseUrl + "?" + rawData + "&vnp_SecureHash=" + secureHash;
            return fullUrl;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rawData = string.Join("&", responseData
                .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                .Select(kv => $"{kv.Key}={kv.Value}"));

            string myChecksum = HmacSHA512(secretKey, rawData);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string HmacSHA512(string key, string inputData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
