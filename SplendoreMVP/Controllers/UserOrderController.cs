using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Repositories;

namespace SplendoreMVP.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly UserOrderRepository _userOrderRepo;

        public UserOrderController(UserOrderRepository userOrderRepo)
        {
            _userOrderRepo = userOrderRepo;
        }

        // GET: /UserOrder/UserOrders
        public async Task<IActionResult> UserOrders()
        {
            var orders = await _userOrderRepo.GetUserOrders();
            return View(orders);
        }

        // GET: /UserOrder/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _userOrderRepo.GetOrderById(id);

            if (order == null)
                return NotFound();

            // eager load OrderItems + Product + Category if not included
            // (your repo currently only uses FindAsync)
            if (order.OrderItems == null || !order.OrderItems.Any())
            {
                // reload with includes
                order = _userOrderRepo
                    .GetUserOrders()
                    .Result // quick fix for now
                    .FirstOrDefault(o => o.Id == id);
            }

            return View(order);
        }
    }
}
