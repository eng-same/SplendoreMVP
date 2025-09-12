using SplendoreMVP.Models;

namespace SplendoreMVP.View_Models
{
    public class ProductDetailsDisplayModel
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<Product> RelatedProducts { get; set; } = new List<Product>();
    }
}
