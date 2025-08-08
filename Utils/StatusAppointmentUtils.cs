using System.Collections.Generic;

namespace pet_spa_system1.Utils;

public static class StatusAppointmentUtils
{
    // Define status progression rules
    private static readonly Dictionary<string, List<string>> ValidStatusTransitions = new()
    {
        // Từ Pending (1) có thể chuyển sang:
        // - Confirmed (2): Khi nhân viên xác nhận
        // - Cancelled (5): Khi bị hủy trực tiếp
        // - PendingCancel (6): Khi khách yêu cầu hủy
        { "Pending", new List<string> { "Confirmed", "Cancelled", "PendingCancel" } },
        
        // Từ Confirmed (2) có thể chuyển sang:
        // - InProgress (3): Khi bắt đầu thực hiện
        // - Cancelled (5): Khi bị hủy
        // - PendingCancel (6): Khi khách yêu cầu hủy
        { "Confirmed", new List<string> { "InProgress", "Cancelled", "PendingCancel" } },
        
        // Từ InProgress (3) chỉ có thể chuyển sang:
        // - Completed (4): Khi hoàn thành
        { "InProgress", new List<string> { "Completed" } },
        
        // Từ PendingCancel (6) chỉ có thể chuyển sang:
        // - Cancelled (5): Khi nhân viên chấp nhận yêu cầu hủy
        { "PendingCancel", new List<string> { "Cancelled" } },
        
        // Các trạng thái cuối cùng không thể chuyển sang trạng thái khác
        { "Completed", new List<string>() },
        { "Cancelled", new List<string>() }
    };

    public static bool IsValidStatusTransition(string currentStatus, string newStatus)
    {
        // Nếu trạng thái hiện tại không tồn tại trong rules, cho phép chuyển
        if (!ValidStatusTransitions.ContainsKey(currentStatus))
            return true;

        // Lấy danh sách các trạng thái có thể chuyển đến
        var validNextStates = ValidStatusTransitions[currentStatus];

        // Kiểm tra xem trạng thái mới có nằm trong danh sách không
        return validNextStates.Contains(newStatus);
    }

    public static List<string> GetValidNextStatuses(string currentStatus)
    {
        if (!ValidStatusTransitions.ContainsKey(currentStatus))
            return new List<string>();

        return ValidStatusTransitions[currentStatus];
    }
}