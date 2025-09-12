using Microsoft.EntityFrameworkCore;
using SplendoreMVP.Data;
using SplendoreMVP.Models;

namespace SplendoreMVP.Repositories
{
    public class UserProductRepository
    {
        private readonly ApplicationDbContext _db;

        public UserProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetRelatedProducts(int categoryId, int excludeProductId, int take = 4)
        {
            return await _db.Products
                .Where(p => p.CategoryID == categoryId && p.Id != excludeProductId)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
