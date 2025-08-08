//using System;
//using pet_spa_system1.Models;
//using pet_spa_system1.Utils;

//namespace pet_spa_system1.ViewModel
//{
//    public class AppointmentServiceViewModel
//    {
//        // Chỉ lưu thông tin cần thiết từ Appointment
//        public int AppointmentId { get; set; }
//        public DateTime AppointmentDate { get; set; }
//        public int UserId { get; set; }
//        public string CustomerName { get; set; } = string.Empty;
//        public int StatusId { get; set; }
//        public string StatusName { get; set; } = string.Empty;

//        // Chỉ lưu thông tin cần thiết từ Service
//        public int ServiceId { get; set; }
//        public string ServiceName { get; set; } = string.Empty;
//        public decimal ServicePrice { get; set; }
//        public decimal Price { get; set; }

//        // Constructor mặc định
//        public AppointmentServiceViewModel() { }

//        // Constructor từ model - sử dụng các tiện ích từ Utils
//        public AppointmentServiceViewModel(AppointmentService model)
//        {
//            if (model == null) return;

//            // Map dữ liệu từ Appointment
//            if (model.Appointment != null)
//            {
//                AppointmentId = model.Appointment.AppointmentId;
//                AppointmentDate = model.Appointment.AppointmentDate;
//                UserId = model.Appointment.UserId;
//                CustomerName = ModelUtils.GetUserFullName(model.Appointment.User);
//                StatusId = model.Appointment.StatusId;
//                StatusName = ModelUtils.GetStatusName(model.Appointment.Status);
//            }

//            // Map dữ liệu từ Service
//            if (model.Service != null)
//            {
//                ServiceId = model.Service.ServiceId;
//                ServiceName = model.Service.Name;
//                ServicePrice = model.Service.Price;
//            }

//            // Giá cụ thể - tìm kiếm các tên thuộc tính tiềm năng
//            Price = ModelUtils.GetDecimalValue(model, "Price", "Amount", "Money", "Cost");
//        }
//    }
//}
