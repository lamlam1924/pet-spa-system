using System;

namespace pet_spa_system1.Models
{
    public class StaffDocument
    {
        public int DocumentId { get; set; }
        public int StaffId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? Description { get; set; }
    }
} 