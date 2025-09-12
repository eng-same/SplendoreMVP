using System.ComponentModel.DataAnnotations;

namespace SplendoreMVP.View_Models
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name must be under 100 characters")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description must be under 500 characters")]
        public string? Description { get; set; }
    }
}
