using SplendoreMVP.Models;

namespace SplendoreMVP.View_Models
{
    public class ProductsDisplayModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public string STerm { get; set; }= string.Empty;
        public int CategoryID { get; set; } //used for search not for relationship 
    }
}
