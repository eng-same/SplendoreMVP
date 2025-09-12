using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Models;
using SplendoreMVP.Repositories;
using SplendoreMVP.View_Models;

namespace SplendoreMVP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOrSupervisor")]
    public class CategoryController : Controller
    {
        private readonly CategoryRepository _categoryRepo;

        public CategoryController(CategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepo.GetCategories();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null)
            {
                // create
                return View(new CategoryVM());
            }

            // edit
            var category = await _categoryRepo.GetCategoryById(id.Value);
            if (category == null)
                return NotFound();

            var vm = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(CategoryVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // CREATE Logic: If the ID is 0, it's a new category.
            if (vm.Id == 0)
            {
                var newCat = new Category
                {
                    Name = vm.Name,
                    Description = vm.Description
                };
                await _categoryRepo.AddCategory(newCat);
            }
            // UPDATE Logic: If the ID is not 0, it's an existing category.
            else
            {
                var exist = await _categoryRepo.GetCategoryById(vm.Id);
                if (exist == null)
                {
                    return NotFound();
                }

                exist.Name = vm.Name;
                exist.Description = vm.Description;
                await _categoryRepo.UpdateCategory(exist);
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryRepo.GetCategoryById(id);
            if (category == null)
                return NotFound();

            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var exist = await _categoryRepo.GetCategoryById(id);
            if (exist == null)
                return NotFound();
            await _categoryRepo.DeleteCategory(id);
            return RedirectToAction("Index");
        }
    }
}
