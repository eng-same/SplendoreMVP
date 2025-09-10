using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Models;
using SplendoreMVP.Repositories;
using SplendoreMVP.Services;
using SplendoreMVP.View_Models;

namespace SplendoreMVP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrSupervisor")]
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepo;
        private readonly IFileService _fileService;
        private readonly CategoryRepository _categoryRepository;
        private readonly IHomeRepository _homerepo;

        public ProductController(ProductRepository productRepo, IFileService fileService, CategoryRepository categoryRepository, IHomeRepository homeRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepo = productRepo;
            _fileService = fileService;
            _homerepo = homeRepository;
        }

        public async Task<IActionResult> Index(string sterm = "", int CategoryID = 0)
        {
            IEnumerable<Product> products = await _homerepo.GetProducts(sterm, CategoryID);
            IEnumerable<Category> categories = await _homerepo.Categories();
            ProductsDisplayModel bookModel = new ProductsDisplayModel
            {
                Products = products,
                Categories = categories,
                STerm = sterm,
                CategoryID = CategoryID
            };
            return View(bookModel);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            ViewBag.Categories = await _categoryRepository.GetCategories();
            if (id == null)
            {
                // Create
                return View(new ProductVM());
            }
            // Edit
            var product = await _productRepo.GetProductById(id.Value);
            if (product == null)
                return NotFound();
            var vm = new ProductVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryID = product.CategoryID
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(int? id, ProductVM vm)
        {
            ViewBag.Categories = await _categoryRepository.GetCategories();
            if (!ModelState.IsValid)
                return View(vm);

            string imageUrl = "/Images/Product/Placeholder.jpg";
            if (vm.Image != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                imageUrl = await _fileService.SaveFile(vm.Image, allowedExtensions);
            }

            if (id == null)
            {
                // Create
                var product = new Product
                {
                    Name = vm.Name,
                    Description = vm.Description,
                    Price = vm.Price,
                    StockQuantity = vm.StockQuantity,
                    CategoryID = vm.CategoryID,
                    ImageUrl = imageUrl
                };
                await _productRepo.AddProduct(product);
            }
            else
            {
                // Update
                var product = await _productRepo.GetProductById(id.Value);
                if (product == null)
                    return NotFound();
                product.Name = vm.Name;
                product.Description = vm.Description;
                product.Price = vm.Price;
                product.StockQuantity = vm.StockQuantity;
                product.CategoryID = vm.CategoryID;
                if (vm.Image != null)
                    product.ImageUrl = imageUrl;
                await _productRepo.Update(product);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productRepo.DeleteProduct(id);
            if (!success)
                return NotFound();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepo.GetProductById(id);
            if (product == null)
                return NotFound();
            return View(product);
        }
    }
}
