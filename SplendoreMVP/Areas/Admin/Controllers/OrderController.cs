using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Models;
using SplendoreMVP.Repositories;

namespace SplendoreMVP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrSupervisor")]
    public class OrderController : Controller
    {
        private readonly OrderRepository _orderRepo;

        public OrderController(OrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepo.GetAllOrders();
            return View(orders);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepo.GetOrderById(id);
            if (order == null) return NotFound();

            return View(order);
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderRepo.GetOrderById(id);
            if (order == null) return NotFound();

            ViewBag.Statuses = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();
            return View(order);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, OrderStatus status)
        {
            await _orderRepo.UpdateOrderStatus(id, status);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepo.GetOrderById(id);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderRepo.DeleteOrder(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
