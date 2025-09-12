namespace SplendoreMVP.View_Models
{
    public class DashboardVM
    {
        // totals
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }

        // chart
        public List<MonthlySalesPoint> MonthlySales { get; set; } = new();

        // stocks
        public List<SplendoreMVP.Models.Product> OutOfStockProducts { get; set; } = new();
        public List<SplendoreMVP.Models.Product> LowStockProducts { get; set; } = new();

        // top sold
        public TopNSoldProductVM TopSoldProducts { get; set; } = new();
    }
}
