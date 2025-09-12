using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SplendoreMVP.Data;
using SplendoreMVP.Models;

namespace SplendoreMVP.Repositories
{
    public class UserOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserOrderRepository(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
             IHttpContextAccessor httpContextAccessor)
        {
            _context = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }


        public async Task<List<Order>> GetUserOrders()
        {
            string userId = GetUserId();
            var orders = _context.Orders
                .Where(o => o.CustomerID == userId)
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return orders;
        }
        public async Task<Order?> GetOrderById(int id)
        {
            string userId = GetUserId();
            return await _context.Orders
                .Where(o => o.Id == id && o.CustomerID == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
        {
            return await _context.Orders
                .Select(o => o.Status)
                .Distinct()
                .ToListAsync();
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }

}
