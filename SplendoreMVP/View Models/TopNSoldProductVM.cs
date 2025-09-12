namespace SplendoreMVP.View_Models
{
    public class TopNSoldProductVM
    {

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int N { get; set; }   // how many top items (ex: Top 5, Top 10)

        public List<TopNSoldProductItemVM> Products { get; set; } = new();
    }
}
