using Inventory_Web.Data;
using Inventory_Web.Models;
using Inventory_Web.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Web.Areas.Seller.Controllers
{
    [Authorize]
    [Area("Seller")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> userManager;
        public HomeController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        private async Task<Sellers> GetSellerId()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var seller = await db.Sellers.FirstOrDefaultAsync(u => u.UserId == user.Id);
            return seller;
        }
        public async Task<IActionResult> Index()
        {
            var seller = await GetSellerId();
            ViewBag.Product = await db.Product.Where(u => u.SellerId == seller.Id).CountAsync();
            ViewBag.Order = await db.Orders.Where(u => u.SellersId == seller.Id).Where(u => u.Status == "paid").CountAsync();
            IQueryable<Orders> item1 = db.Orders.Include(p => p.Products).AsQueryable();
            IQueryable<Products> item2 = db.Product.Include(p => p.Category).AsQueryable();
            List<Orders> order = await item1.Take(3).OrderBy(p => p.Name).ToListAsync();
            List<Products> product = await item2.Take(3).OrderBy(p => p.Id).ToListAsync();
            var home = new HomeVM
            {
                orders = order,
                product = product
            };
            return View(home);
        }
    }
}
