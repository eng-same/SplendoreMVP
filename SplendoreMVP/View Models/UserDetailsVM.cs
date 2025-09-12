using SplendoreMVP.Models;

namespace SplendoreMVP.View_Models
{
    public class UserDetailsVM
    {
        public ApplicationUser User { get; set; } = null!;
        public List<Order> Orders { get; set; } = new();
    }
}
