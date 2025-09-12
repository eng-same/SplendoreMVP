namespace SplendoreMVP.View_Models
{
    public class MonthlySalesPoint
    {   public int Year { get; set; }
        public int Month { get; set; } // 1..12
        public decimal Total { get; set; }
        public string Label => new DateTime(Year, Month, 1).ToString("MMM yyyy");
    }
}
