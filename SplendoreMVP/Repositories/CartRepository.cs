using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SplendoreMVP.Data;
using SplendoreMVP.Models;
using SplendoreMVP.View_Models;
using System.Net;

namespace SplendoreMVP.Repositories
{
    [Authorize]
    public class CartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CartRepository(ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }



        public async Task<int> AddItem(int productId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new Cart
                    {
                        CustomerID = userId
                    };
                    _context.Carts.Add(cart);
                }
                _context.SaveChanges();
                // cart detail section
                var cartItem = _context.CartItems
                                  .FirstOrDefault(a => a.CartID == cart.Id && a.ProductID == productId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _context.Products.Find(productId);
                    cartItem = new CartItem
                    {
                        ProductID = productId,
                        CartID = cart.Id,
                        Quantity = qty
                    };
                    _context.CartItems.Add(cartItem);
                }
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex) { 
            
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }




        public async Task<Cart?> GetUserCart()
        {
            try
            {
                var userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                    throw new InvalidOperationException("No logged-in user.");

                var shoppingCart = await _context.Carts
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(c => c.CustomerID == userId);

                return shoppingCart;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }




        public async Task<Cart> GetCart(string userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.CustomerID == userId);
            return cart;
        }





        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId)) // updated line
            {
                userId = GetUserId();
            }
            var data = await (from cart in _context.Carts
                              join cartItem in _context.CartItems
                              on cart.Id equals cartItem.CartID
                              where cart.CustomerID == userId // updated line
                              select new { cartItem.Id }
                        ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout() //CheckoutVM model add later and update order model
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1️⃣ Get the current user ID
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged in");

                // 2️⃣ Get the user's cart with items and products
                var cart = await _context.Carts
                                         .Include(c => c.Items)
                                             .ThenInclude(ci => ci.Product)
                                         .FirstOrDefaultAsync(c => c.CustomerID == userId);

                if (cart == null || !cart.Items.Any())
                    throw new InvalidOperationException("Cart is empty");

                // 3️⃣ Calculate total amount
                decimal totalAmount = cart.Items.Sum(i => i.Quantity * i.Product.Price);

                // 4️⃣ Create a new order
                var order = new Order
                {
                    CustomerID = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = totalAmount
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                // 5️⃣ Move cart items to order items and update product stock
                foreach (var item in cart.Items)
                {
                    var product = item.Product;

                    if (product == null)
                        throw new InvalidOperationException("Product not found");

                    if (item.Quantity > product.StockQuantity)
                        throw new InvalidOperationException($"Only {product.StockQuantity} item(s) are available for {product.Name}");

                    // Create order item
                    var orderItem = new OrderItem
                    {
                        OrderID = order.Id,
                        ProductID = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    await _context.OrderItems.AddAsync(orderItem);

                    // Decrease stock
                    product.StockQuantity -= item.Quantity;
                }

                // 6️⃣ Remove cart items
                _context.CartItems.RemoveRange(cart.Items);

                // 7️⃣ Save all changes and commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Optional: log exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<int> RemoveItem(int productId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not logged in");

            try
            {
                // 1️⃣ Get user's cart
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.CustomerID == userId);

                if (cart == null)
                    throw new InvalidOperationException("Cart not found");

                // 2️⃣ Find the cart item by ProductID
                var cartItem = cart.Items.FirstOrDefault(i => i.ProductID == productId);

                if (cartItem == null)
                    throw new InvalidOperationException("Item not found in cart");

                // 3️⃣ Update or remove
                if (cartItem.Quantity <= 1)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity -= 1;
                }

                // 4️⃣ Save changes
                await _context.SaveChangesAsync();

                // 5️⃣ Return updated cart count
                return await GetCartItemCount(userId);
            }
            catch (Exception ex)
            {
                // optional: log exception
                Console.WriteLine(ex.Message);
                return 0; // return safe default
            }
        }


        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}
