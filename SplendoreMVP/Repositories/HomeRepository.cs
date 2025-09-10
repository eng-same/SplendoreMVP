using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using SplendoreMVP.Data;
using SplendoreMVP.Models;

namespace SplendoreMVP.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> Categories()
        {
            return await _db.Categories.ToListAsync();
        }
        public async Task<List<Product>> GetProducts(string sTerm = "", int categoryId = 0)
        {
            var productsQuery = _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId > 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryID == categoryId);
            }

            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(sTerm) ||
                    (p.Description != null && p.Description.Contains(sTerm)));
            }

            return await productsQuery.ToListAsync();
        }
    }
}
