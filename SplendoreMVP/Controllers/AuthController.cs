using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SplendoreMVP.Models;
using SplendoreMVP.Services;
using SplendoreMVP.View_Models;

namespace SplendoreMVP.Controllers
{
    public class AuthController : Controller
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFileService _fileService;

        public AuthController(SignInManager<ApplicationUser> signInManager,
                              UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager ,
                              IFileService fileService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _fileService = fileService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Register(UserRegesterVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                ModelState.AddModelError("", "User already exists");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName =model.FirstName ,
                    LastName =model.LastName 
                };
                if(model.ProfilePic != null)
                {
                    var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
                    var profilePicName = await _fileService.SaveFile(model.ProfilePic, allowedExtensions);
                    newUser.ProfilePic = profilePicName;
                    Console.WriteLine("\n \n add image to the server \n \n  ");
                }
                    
               
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "Customer");
                    await _signInManager.SignInAsync(newUser, isPersistent: model.RememberMe);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        { 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password, 
                isPersistent: model.RememberMe,
                lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user , "Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin", new {Area ="Admin"});

                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
