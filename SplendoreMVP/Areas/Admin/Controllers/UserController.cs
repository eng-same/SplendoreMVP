using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Models;
using SplendoreMVP.View_Models;
using SplendoreMVP.Repositories;

namespace SplendoreMVP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrSupervisor")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly OrderRepository _orderRepo;

        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            OrderRepository orderRepo)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _orderRepo = orderRepo;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var userList = new List<UserWithRolesVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserWithRolesVM
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    UserName = user.UserName,
                    ProfilePic = user.ProfilePic,
                    Roles = roles.ToList()
                });
            }

            return View(userList);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var orders = await _orderRepo.GetAllOrders();
            var userOrders = orders.Where(o => o.Customer.Id == id).ToList();

            var vm = new UserDetailsVM
            {
                User = user,
                Orders = userOrders
            };

            return View(vm);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}

