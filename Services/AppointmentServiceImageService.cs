using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using pet_spa_system1.Models;

namespace pet_spa_system1.Services
{
    /// <summary>
    /// Service quản lý ảnh Before/After cho AppointmentService
    /// </summary>
    public class AppointmentServiceImageService : IAppointmentServiceImageService
    {
        private readonly PetDataShopContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public AppointmentServiceImageService(
            PetDataShopContext context,
            ICloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Upload ảnh Before/After cho AppointmentService
        /// PetId sẽ được auto-detect từ staff assignment (1 staff = 1 pet)
        /// </summary>
        public async Task<ServiceResult<AppointmentServiceImageResult>> UploadImageAsync(
            int appointmentServiceId,
            IFormFile imageFile,
            string photoType,
            int staffId)
        {
            try
            {
                // Validate input
                if (imageFile == null || imageFile.Length == 0)
                {
                    return ServiceResult<AppointmentServiceImageResult>.ErrorResult("Vui lòng chọn file ảnh");
                }

                if (photoType != "Before" && photoType != "After")
                {
                    return ServiceResult<AppointmentServiceImageResult>.ErrorResult("Loại ảnh không hợp lệ");
                }

                // Kiểm tra quyền truy cập
                var hasAccess = await HasAccessToAppointmentServiceAsync(appointmentServiceId, staffId);
                if (!hasAccess)
                {
                    return ServiceResult<AppointmentServiceImageResult>.ErrorResult("Không có quyền upload ảnh cho dịch vụ này");
                }

                // Kiểm tra AppointmentService có tồn tại không và lấy thông tin appointment
                var appointmentService = await _context.AppointmentServices
                    .Include(aps => aps.Service)
                    .Include(aps => aps.Appointment)
                        .ThenInclude(a => a.AppointmentPets)
                    .FirstOrDefaultAsync(aps => aps.AppointmentServiceId == appointmentServiceId);

                if (appointmentService == null)
                {
                    return ServiceResult<AppointmentServiceImageResult>.ErrorResult("Không tìm thấy dịch vụ");
                }

                // Upload ảnh lên Cloudinary
                var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "appointment-services");

                if (string.IsNullOrEmpty(imageUrl))
                {
                    return ServiceResult<AppointmentServiceImageResult>.ErrorResult("Không thể upload ảnh lên server");
                }

                // Auto-detect PetId từ staff assignment (1 staff chỉ phục vụ 1 pet)
                var assignedPet = appointmentService.Appointment.AppointmentPets?
                    .FirstOrDefault(ap => ap.StaffId == staffId);

                int? finalPetId = null;

                if (assignedPet != null)
                {
                    finalPetId = assignedPet.PetId;
                    Console.WriteLine($"[DEBUG] UploadImageAsync: Auto-detected petId: {finalPetId} for staffId: {staffId}");
                }
                else
                {
                    // Fallback: Nếu staff không được assign pet cụ thể, lấy pet đầu tiên
                    finalPetId = appointmentService.Appointment.AppointmentPets?.FirstOrDefault()?.PetId;
                    Console.WriteLine($"[DEBUG] UploadImageAsync: No pet assigned to staffId: {staffId}, using first pet: {finalPetId}");
                }

                Console.WriteLine($"[DEBUG] UploadImageAsync: Final PetId={finalPetId} for appointmentServiceId={appointmentServiceId}");

                // Lưu thông tin ảnh vào database
                var serviceImage = new AppointmentServiceImage
                {
                    AppointmentServiceId = appointmentServiceId,
                    ImgUrl = imageUrl,
                    PhotoType = photoType,
                    PetId = finalPetId, // Set PetId (có thể từ parameter hoặc auto-detect)
                    CreatedAt = DateTime.Now
                };

                _context.AppointmentServiceImages.Add(serviceImage);
                await _context.SaveChangesAsync();

                // Tạo result
                var result = new AppointmentServiceImageResult
                {
                    ImageId = serviceImage.ImageId,
                    AppointmentServiceId = appointmentServiceId,
                    ImageUrl = imageUrl,
                    PhotoType = photoType,
                    PetId = finalPetId, // Include PetId in result
                    CreatedAt = serviceImage.CreatedAt
                };

                return ServiceResult<AppointmentServiceImageResult>.SuccessResult(
                    result, 
                    $"Upload ảnh {photoType} thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading service image: {ex.Message}");
                return ServiceResult<AppointmentServiceImageResult>.ErrorResult("Có lỗi xảy ra khi upload ảnh");
            }
        }

        /// <summary>
        /// Lấy danh sách ảnh của AppointmentService
        /// </summary>
        public async Task<ServiceResult<List<AppointmentServiceImageResult>>> GetImagesAsync(
            int appointmentServiceId, 
            int staffId)
        {
            try
            {
                // Kiểm tra quyền truy cập
                var hasAccess = await HasAccessToAppointmentServiceAsync(appointmentServiceId, staffId);
                if (!hasAccess)
                {
                    return ServiceResult<List<AppointmentServiceImageResult>>.ErrorResult("Không có quyền xem ảnh của dịch vụ này");
                }

                // Lấy danh sách ảnh
                var images = await _context.AppointmentServiceImages
                    .Where(img => img.AppointmentServiceId == appointmentServiceId)
                    .OrderBy(img => img.PhotoType)
                    .ThenBy(img => img.CreatedAt)
                    .Select(img => new AppointmentServiceImageResult
                    {
                        ImageId = img.ImageId,
                        AppointmentServiceId = img.AppointmentServiceId,
                        ImageUrl = img.ImgUrl,
                        PhotoType = img.PhotoType ?? "Before",
                        PetId = img.PetId, // Include PetId
                        CreatedAt = img.CreatedAt
                    })
                    .ToListAsync();

                return ServiceResult<List<AppointmentServiceImageResult>>.SuccessResult(
                    images, 
                    "Lấy danh sách ảnh thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting service images: {ex.Message}");
                return ServiceResult<List<AppointmentServiceImageResult>>.ErrorResult("Có lỗi xảy ra khi lấy danh sách ảnh");
            }
        }

        /// <summary>
        /// Xóa ảnh AppointmentService
        /// </summary>
        public async Task<ServiceResult<bool>> DeleteImageAsync(int imageId, int staffId)
        {
            try
            {
                var serviceImage = await _context.AppointmentServiceImages
                    .Include(img => img.AppointmentService)
                        .ThenInclude(aps => aps.Appointment)
                            .ThenInclude(a => a.AppointmentPets)
                    .FirstOrDefaultAsync(img => img.ImageId == imageId);

                if (serviceImage == null)
                {
                    return ServiceResult<bool>.ErrorResult("Không tìm thấy ảnh");
                }

                // Kiểm tra quyền
                var hasAccess = await HasAccessToAppointmentServiceAsync(serviceImage.AppointmentServiceId, staffId);
                if (!hasAccess)
                {
                    return ServiceResult<bool>.ErrorResult("Không có quyền xóa ảnh này");
                }

                // Xóa ảnh trên Cloudinary
                await _cloudinaryService.DeleteImageAsync(serviceImage.ImgUrl);

                // Xóa record trong database
                _context.AppointmentServiceImages.Remove(serviceImage);
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.SuccessResult(true, "Xóa ảnh thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting service image: {ex.Message}");
                return ServiceResult<bool>.ErrorResult("Có lỗi xảy ra khi xóa ảnh");
            }
        }

        /// <summary>
        /// Lấy danh sách ảnh của AppointmentService cho customer
        /// </summary>
        public async Task<ServiceResult<List<AppointmentServiceImageResult>>> GetImagesForCustomerAsync(
            int appointmentServiceId,
            int customerId)
        {
            try
            {
                Console.WriteLine($"[DEBUG] GetImagesForCustomerAsync: appointmentServiceId={appointmentServiceId}, customerId={customerId}");

                // Kiểm tra customer có quyền xem ảnh này không
                var appointmentService = await _context.AppointmentServices
                    .Include(aps => aps.Appointment)
                    .FirstOrDefaultAsync(aps => aps.AppointmentServiceId == appointmentServiceId);

                Console.WriteLine($"[DEBUG] GetImagesForCustomerAsync: appointmentService found={appointmentService != null}");

                if (appointmentService == null)
                {
                    Console.WriteLine($"[DEBUG] GetImagesForCustomerAsync: AppointmentService not found");
                    return ServiceResult<List<AppointmentServiceImageResult>>.ErrorResult("Không tìm thấy dịch vụ");
                }

                Console.WriteLine($"[DEBUG] GetImagesForCustomerAsync: appointment.UserId={appointmentService.Appointment.UserId}, customerId={customerId}");

                if (appointmentService.Appointment.UserId != customerId)
                {
                    Console.WriteLine($"[DEBUG] GetImagesForCustomerAsync: Permission denied");
                    return ServiceResult<List<AppointmentServiceImageResult>>.ErrorResult("Không có quyền xem ảnh này");
                }

                // Lấy danh sách ảnh
                var images = await _context.AppointmentServiceImages
                    .Where(img => img.AppointmentServiceId == appointmentServiceId)
                    .OrderBy(img => img.PhotoType)
                    .ThenBy(img => img.CreatedAt)
                    .Select(img => new AppointmentServiceImageResult
                    {
                        ImageId = img.ImageId,
                        AppointmentServiceId = img.AppointmentServiceId,
                        ImageUrl = img.ImgUrl,
                        PhotoType = img.PhotoType ?? "Before",
                        PetId = img.PetId, // Include PetId
                        CreatedAt = img.CreatedAt
                    })
                    .ToListAsync();

                Console.WriteLine($"[DEBUG] GetImagesForCustomerAsync: Found {images.Count} images");
                foreach (var img in images)
                {
                    Console.WriteLine($"[DEBUG] Image: Id={img.ImageId}, Type={img.PhotoType}, PetId={img.PetId}, Url={img.ImageUrl}");
                }

                return ServiceResult<List<AppointmentServiceImageResult>>.SuccessResult(
                    images,
                    "Lấy danh sách ảnh thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Error getting service images for customer: {ex.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
                return ServiceResult<List<AppointmentServiceImageResult>>.ErrorResult("Có lỗi xảy ra khi lấy danh sách ảnh");
            }
        }

        /// <summary>
        /// Kiểm tra staff có quyền truy cập AppointmentService không
        /// </summary>
        public async Task<bool> HasAccessToAppointmentServiceAsync(int appointmentServiceId, int staffId)
        {
            try
            {
                var appointmentService = await _context.AppointmentServices
                    .Include(aps => aps.Appointment)
                        .ThenInclude(a => a.AppointmentPets)
                    .FirstOrDefaultAsync(aps => aps.AppointmentServiceId == appointmentServiceId);

                if (appointmentService == null)
                {
                    return false;
                }

                // Check if staff is assigned to this appointment through AppointmentPet
                return appointmentService.Appointment.AppointmentPets
                    .Any(ap => ap.StaffId == staffId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking access to appointment service: {ex.Message}");
                return false;
            }
        }
    }
}
