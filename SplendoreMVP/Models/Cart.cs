namespace SplendoreMVP.Models
{
    public class Cart
    {
        public int Id { get; set; }

        //FK
        public string CustomerID { get; set; }

        //Navigation
        public ApplicationUser Customer { get; set; } = null!;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
