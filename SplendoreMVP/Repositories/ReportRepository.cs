using Microsoft.EntityFrameworkCore;
using SplendoreMVP.Data;
using SplendoreMVP.View_Models;

namespace SplendoreMVP.Repositories
{
    public class ReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TopNSoldProductVM> GetTopNSoldProducts(DateTime startDate, DateTime endDate, int n)
        {
            var topProducts = await _context.OrderItems
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => new { oi.ProductID, oi.Product.Name, oi.Product.ImageUrl })
                .Select(g => new TopNSoldProductItemVM
                {
                    ProductId = g.Key.ProductID,
                    ProductName = g.Key.Name,
                    ImageUrl = g.Key.ImageUrl,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(n)
                .ToListAsync();

            return new TopNSoldProductVM
            {
                StartDate = startDate,
                EndDate = endDate,
                N = n,
                Products = topProducts
            };
        }

        public async Task<DashboardVM> GetDashboardAsync(
    int months = 6,
    int topNProducts = 5,
    int lowStockThreshold = 5)
        {
            var now = DateTime.UtcNow;
            var startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-(months - 1));
            var endDate = now;

            // totals (sequential)
            var totalOrders = await _context.Orders.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalRevenue = await _context.Orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            // monthly sales grouping
            var monthlyRaw = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new MonthlySalesPoint
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync();

            // fill missing months
            var monthlyList = new List<MonthlySalesPoint>();
            for (int i = 0; i < months; i++)
            {
                var dt = startDate.AddMonths(i);
                var found = monthlyRaw.FirstOrDefault(m => m.Year == dt.Year && m.Month == dt.Month);
                monthlyList.Add(found ?? new MonthlySalesPoint { Year = dt.Year, Month = dt.Month, Total = 0m });
            }

            // out of stock
            var outOfStock = await _context.Products
                .Where(p => p.StockQuantity == 0)
                .OrderBy(p => p.Name)
                .ToListAsync();

            // low stock
            var lowStock = await _context.Products
                .Where(p => p.StockQuantity > 0 && p.StockQuantity <= lowStockThreshold)
                .OrderBy(p => p.StockQuantity)
                .Take(5)
                .ToListAsync();

            // top sold
            var topSold = await GetTopNSoldProducts(startDate, endDate, topNProducts);

            return new DashboardVM
            {
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                TotalRevenue = totalRevenue,
                MonthlySales = monthlyList,
                OutOfStockProducts = outOfStock,
                LowStockProducts = lowStock,
                TopSoldProducts = topSold
            };
        }
    }
}
