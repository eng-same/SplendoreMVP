namespace SplendoreMVP.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        //FK
        public int OrderID { get; set; }
        public int ProductID { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
