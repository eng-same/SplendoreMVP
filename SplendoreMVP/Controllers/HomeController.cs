using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Models;
using SplendoreMVP.View_Models;
using SplendoreMVP.Repositories;
using System.Diagnostics;

namespace SplendoreMVP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homerepo;
        private readonly UserProductRepository _productrepo;

        public HomeController(ILogger<HomeController> logger ,IHomeRepository homeRepository , UserProductRepository userProductRepository)
        {
            _logger = logger;
            _homerepo = homeRepository;
            _productrepo = userProductRepository;
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

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productrepo.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var related = await _productrepo.GetRelatedProducts(product.CategoryID, product.Id);

            var vm = new ProductDetailsDisplayModel
            {
                Product = product,
                RelatedProducts = related
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Access Denied";
            return View();
        }
    }
}
