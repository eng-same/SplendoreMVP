using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Repositories;

namespace SplendoreMVP.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportRepository _reportRepo;
        public ReportsController(ReportRepository reportRepository)
        {
            _reportRepo = reportRepository;
        }

        // Show Top N Sold Products
        public async Task<IActionResult> Index(int n = 3, DateTime? sDate = null, DateTime? eDate = null)
        {
            // Default date range: last 30 days if not provided
            var startDate = sDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = eDate ?? DateTime.UtcNow;

            var model = await _reportRepo.GetTopNSoldProducts(startDate, endDate, n);

            return View(model);
        }
    }
}
