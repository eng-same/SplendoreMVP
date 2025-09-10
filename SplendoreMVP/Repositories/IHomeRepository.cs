using SplendoreMVP.Models;

namespace SplendoreMVP.Repositories
{
    public interface IHomeRepository
    {
        public Task<IEnumerable<Category>> Categories();
        public Task<List<Product>> GetProducts(string sTerm = "", int categoryId = 0);
    }
}