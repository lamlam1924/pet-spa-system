using System.Text.RegularExpressions;

namespace pet_spa_system1.Utils
{
    public static class ValidationInput
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Đơn giản, có thể thay thế bằng regex mạnh hơn nếu cần
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            // Chấp nhận số bắt đầu bằng 0, 10-11 số, chỉ số
            var pattern = @"^(0[0-9]{9,10})$";
            return Regex.IsMatch(phone, pattern);
        }

        public static bool IsValidBookingDate(DateTime bookingDate)
        {
            // Ngày đặt lịch phải lớn hơn hoặc bằng ngày hiện tại (không cho phép đặt lịch quá khứ)
            return bookingDate.Date >= DateTime.Now.Date;
        }
    }
}
