using Microsoft.EntityFrameworkCore;
using SplendoreMVP.Data;
using SplendoreMVP.Models;
namespace SplendoreMVP.Repositories
{
    public class ProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task Update(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product?> GetProductById(int id) => await _context.Products.FindAsync(id);

        public async Task<IEnumerable<Product>> GetProducts() => await _context.Products.Include(a => a.Category).ToListAsync();
    }
}
