using Microsoft.AspNetCore.Http;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    /// <summary>
    /// Interface cho AppointmentServiceImage Service
    /// Quản lý upload, view, delete ảnh Before/After cho dịch vụ
    /// </summary>
    public interface IAppointmentServiceImageService
    {
        /// <summary>
        /// Upload ảnh Before/After cho AppointmentService
        /// PetId sẽ được auto-detect từ staff assignment (1 staff = 1 pet)
        /// </summary>
        /// <param name="appointmentServiceId">ID của AppointmentService</param>
        /// <param name="imageFile">File ảnh upload</param>
        /// <param name="photoType">Loại ảnh: "Before" hoặc "After"</param>
        /// <param name="staffId">ID của staff thực hiện upload</param>
        /// <returns>Result với thông tin ảnh đã upload</returns>
        Task<ServiceResult<AppointmentServiceImageResult>> UploadImageAsync(
            int appointmentServiceId,
            IFormFile imageFile,
            string photoType,
            int staffId);

        /// <summary>
        /// Lấy danh sách ảnh của AppointmentService
        /// </summary>
        /// <param name="appointmentServiceId">ID của AppointmentService</param>
        /// <param name="staffId">ID của staff yêu cầu xem</param>
        /// <returns>Danh sách ảnh</returns>
        Task<ServiceResult<List<AppointmentServiceImageResult>>> GetImagesAsync(
            int appointmentServiceId, 
            int staffId);

        /// <summary>
        /// Xóa ảnh AppointmentService
        /// </summary>
        /// <param name="imageId">ID của ảnh cần xóa</param>
        /// <param name="staffId">ID của staff thực hiện xóa</param>
        /// <returns>Kết quả xóa</returns>
        Task<ServiceResult<bool>> DeleteImageAsync(int imageId, int staffId);

        /// <summary>
        /// Kiểm tra staff có quyền truy cập AppointmentService không
        /// </summary>
        /// <param name="appointmentServiceId">ID của AppointmentService</param>
        /// <param name="staffId">ID của staff</param>
        /// <returns>True nếu có quyền</returns>
        Task<bool> HasAccessToAppointmentServiceAsync(int appointmentServiceId, int staffId);

        /// <summary>
        /// Lấy danh sách ảnh của AppointmentService cho customer
        /// </summary>
        /// <param name="appointmentServiceId">ID của AppointmentService</param>
        /// <param name="customerId">ID của customer</param>
        /// <returns>Danh sách ảnh</returns>
        Task<ServiceResult<List<AppointmentServiceImageResult>>> GetImagesForCustomerAsync(
            int appointmentServiceId,
            int customerId);
    }

    /// <summary>
    /// Result model cho AppointmentServiceImage operations
    /// </summary>
    public class AppointmentServiceImageResult
    {
        public int ImageId { get; set; }
        public int AppointmentServiceId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string PhotoType { get; set; } = string.Empty;
        public int? PetId { get; set; } // Thêm PetId để group ảnh theo pet
        public DateTime CreatedAt { get; set; }
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    }

    /// <summary>
    /// Generic service result class
    /// </summary>
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResult<T> SuccessResult(T data, string message = "")
        {
            return new ServiceResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResult<T> ErrorResult(string message)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                Data = default(T)
            };
        }
    }
}
