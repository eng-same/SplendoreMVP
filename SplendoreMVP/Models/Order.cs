namespace SplendoreMVP.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        //FK
        public int CustomerID { get; set; }

        // Navigation
        public ApplicationUser Customer { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

