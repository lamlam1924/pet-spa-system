using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace pet_spa_system1.ViewModel
{
    public class BlogCreateViewModel
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Danh mục không được vượt quá 100 ký tự")]
        public string? Category { get; set; }

        public string? FeaturedImageUrl { get; set; }

        public string Status { get; set; } = "Draft";

        public IFormFile? FeaturedImage { get; set; }
        public List<IFormFile>? AdditionalImages { get; set; }

        public List<string>? Categories { get; set; }
        public List<string> AvailableCategories { get; set; } = new();

        public int BlogId { get; set; }
    }
}