using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.Utils;
using System.Threading.Tasks;

namespace pet_spa_system1.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaffService(IStaffRepository staffRepository, IHttpContextAccessor httpContextAccessor)
        {
            _staffRepository = staffRepository;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<User> GetCurrentStaffProfileAsync()
        {
            var currentUser = _httpContextAccessor.HttpContext?.Session.GetObjectFromJson<User>("CurrentUser");
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
            }
            return await _staffRepository.GetStaffProfileByIdAsync(currentUser.UserId);
        }

        public async Task<User> SaveStaffProfileChangesAsync(User user)
        {
            var currentUser = await GetCurrentStaffProfileAsync();
            currentUser.FullName = user.FullName ?? currentUser.FullName;
            currentUser.Phone = user.Phone ?? currentUser.Phone;
            currentUser.Address = user.Address ?? currentUser.Address;
            currentUser.ProfilePictureUrl = user.ProfilePictureUrl ?? currentUser.ProfilePictureUrl;
            currentUser.UpdatedAt = DateTime.Now;

            await _staffRepository.UpdateStaffProfileAsync(currentUser);
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Session.SetObjectAsJson("CurrentUser", currentUser);
            }
            return currentUser;
        }
    }
}