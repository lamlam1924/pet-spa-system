namespace pet_spa_system1.ViewModel
{
    public class StaffViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public bool IsBusy { get; set; } // Trạng thái bận/rảnh
        public string? ProfilePictureUrl { get; set; } // Ảnh đại diện nhân viên
    }
}
