using Microsoft.AspNetCore.Identity;

namespace SplendoreMVP.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName {  get; set; }
        
        public string LastName { get; set; }

        public string? ProfilePic {  get; set; }

        //Navigation
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public Cart? Cart { get; set; }
    }
}
