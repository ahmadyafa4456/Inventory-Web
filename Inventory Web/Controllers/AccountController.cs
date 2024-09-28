using Inventory_Web.Models;
using Inventory_Web.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signManager;
        public AccountController(SignInManager<AppUser> signManager, UserManager<AppUser> userManager)
        {
            this.signManager = signManager;
            this.userManager = userManager;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return RedirectToAction("Index", "Home", new { area = "Seller" });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(model.Name);
                if (user != null)
                {
                    var result = await signManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        await signManager.SignInAsync(user, false);
                        if (await userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                        return RedirectToAction("Index", "Home", new { area = "Seller" });
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not registered");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
