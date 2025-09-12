using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Repositories;
using SplendoreMVP.View_Models;

namespace SplendoreMVP.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly CartRepository _cartRepo;

        public CartController(CartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        public async Task<IActionResult> AddItem(int productId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(productId, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int productId)
        {
            var cartCount = await _cartRepo.RemoveItem(productId);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }

        //public IActionResult Checkout()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> Checkout() //add parameter later CheckoutVM model
        {
            //if (!ModelState.IsValid)
            //    return View(model);
            bool isCheckedOut = await _cartRepo.DoCheckout();
            if (!isCheckedOut)
                return RedirectToAction(nameof(OrderFailure));
            return RedirectToAction(nameof(OrderSuccess));
        }
        public async Task<IActionResult> Buy(int id)
        {
            try
            {
                // Step 1: Add the product to the cart (quantity = 1)
                await _cartRepo.AddItem(id, 1);

                // Step 2: Directly checkout
                bool isCheckedOut = await _cartRepo.DoCheckout();

                if (!isCheckedOut)
                    return RedirectToAction(nameof(OrderFailure));

                // Step 3: Redirect to success page
                return RedirectToAction(nameof(OrderSuccess));
            }
            catch (Exception ex)
            {
                // Optional: log exception
                return RedirectToAction(nameof(OrderFailure));
            }
        }


        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }

    }
}
