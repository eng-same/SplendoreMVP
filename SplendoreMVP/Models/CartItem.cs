namespace SplendoreMVP.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        //FK
        public int CartID { get; set; }
        public int ProductID { get; set; }

        // Navigation
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;

    }
}