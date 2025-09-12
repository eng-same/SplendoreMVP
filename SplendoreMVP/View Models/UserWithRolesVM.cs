namespace SplendoreMVP.View_Models
{
    public class UserWithRolesVM
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? ProfilePic { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}

