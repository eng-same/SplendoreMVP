using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Repositories;

namespace SplendoreMVP.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ReportRepository _reportRepo;
        private readonly ProductRepository _productRepo;

        public AdminController(ReportRepository reportRepo, ProductRepository productRepo)
        {
            _reportRepo = reportRepo;
            _productRepo = productRepo;
        }

        public async Task<IActionResult> Dashboard()
        {
            // fetch last 12 months, top 5 products, low-stock threshold 5
            var vm = await _reportRepo.GetDashboardAsync(months: 12, topNProducts: 5, lowStockThreshold: 5);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickUpdateStock(int id, int stock)
        {
            var product = await _productRepo.GetProductById(id);
            if (product == null) return NotFound();

            product.StockQuantity = stock;
            await _productRepo.Update(product);

            return Json(new { success = true, id = product.Id, stock = product.StockQuantity });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickUpdatePrice(int id, decimal price)
        {
            var product = await _productRepo.GetProductById(id);
            if (product == null) return NotFound();

            product.Price = price;
            await _productRepo.Update(product);

            return Json(new { success = true, id = product.Id, price = product.Price });
        }
    }
}
