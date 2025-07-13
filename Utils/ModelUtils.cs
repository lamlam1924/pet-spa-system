using System;
using System.Linq;
using System.Reflection;
using pet_spa_system1.Models;

namespace pet_spa_system1.Utils
{
    public static class ModelUtils
    {
        // Phương thức trợ giúp để lấy giá trị decimal từ model
        public static decimal GetDecimalValue(object model, params string[] possiblePropertyNames)
        {
            if (model == null) return 0;
            
            var properties = model.GetType().GetProperties();
            foreach (var propName in possiblePropertyNames)
            {
                var prop = properties.FirstOrDefault(p => p.Name == propName);
                if (prop != null)
                {
                    var value = prop.GetValue(model);
                    if (value is decimal decimalValue)
                    {
                        return decimalValue;
                    }
                    // Sửa lỗi pattern matching
                    else if (value is decimal?)
                    {
                        return ((decimal?)value).GetValueOrDefault();
                    }
                }
            }
            return 0;
        }
        
        // Phương thức trợ giúp để lấy giá trị chuỗi từ model
        public static string GetStringValue(object model, params string[] possiblePropertyNames)
        {
            if (model == null) return string.Empty;
            
            var properties = model.GetType().GetProperties();
            foreach (var propName in possiblePropertyNames)
            {
                var prop = properties.FirstOrDefault(p => p.Name == propName);
                if (prop != null)
                {
                    var value = prop.GetValue(model);
                    if (value is string stringValue)
                    {
                        return stringValue;
                    }
                }
            }
            return string.Empty;
        }
        
        // Phương thức trợ giúp để lấy ngày từ model
        public static DateTime? GetDateTimeValue(object model, params string[] possiblePropertyNames)
        {
            if (model == null) return null;
            
            var properties = model.GetType().GetProperties();
            foreach (var propName in possiblePropertyNames)
            {
                var prop = properties.FirstOrDefault(p => p.Name == propName);
                if (prop != null)
                {
                    var value = prop.GetValue(model);
                    if (value is DateTime dtValue)
                    {
                        return dtValue;
                    }
                    // Sửa lỗi pattern matching
                    else if (value is DateTime?)
                    {
                        return (DateTime?)value;
                    }
                }
            }
            return null;
        }
        
        // Phương thức tiện ích để lấy tên đầy đủ người dùng
        public static string GetUserFullName(User? user)
        {
            if (user == null) return "Không xác định";
            
            // Thử lấy FullName trước
            string fullName = GetStringValue(user, "FullName", "Name", "DisplayName");
            if (!string.IsNullOrEmpty(fullName))
                return fullName;
            
            // Thử kết hợp FirstName và LastName
            string firstName = GetStringValue(user, "FirstName", "GivenName");
            string lastName = GetStringValue(user, "LastName", "Surname", "FamilyName");
            
            if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                return $"{firstName} {lastName}";
            else if (!string.IsNullOrEmpty(firstName))
                return firstName;
            else if (!string.IsNullOrEmpty(lastName))
                return lastName;
            
            return "Không xác định";
        }
        
        // Phương thức tiện ích để lấy tên trạng thái
        public static string GetStatusName(StatusAppointment? status)
        {
            if (status == null) return "Không xác định";
            
            return GetStringValue(status, "StatusName", "Name", "Description");
        }
        
        // Phương thức tiện ích để lấy giá từ AppointmentService
        public static decimal GetAppointmentServicePrice(AppointmentService appointmentService)
        {
            if (appointmentService == null) return 0;
            
            return GetDecimalValue(appointmentService, "Price", "Money", "Amount", "Cost");
        }
    }
}