using Inventory_Web.Data;
using Inventory_Web.Models;
using Inventory_Web.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Web.Areas.Seller.Controllers
{
    [Authorize]
    [Area("Seller")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> userManager;
        public OrderController(ApplicationDbContext db, UserManager<AppUser> userManager)
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
            Sellers seller = await GetSellerId();
            if (seller == null)
            {
                return NotFound();
            }
            List<Orders> orders = await db.Orders.Where(u => u.SellersId == seller.Id).Include(u => u.Products).ToListAsync();
            if (orders != null)
            {
                return View(orders);
            }
            return View();
        }
        public async Task<IActionResult> Create()
        {
            Sellers seller = await GetSellerId();
            var productList = await db.Product.Where(u => u.SellerId == seller.Id).Select(u => new SelectListItem
            {
                Text = u.title,
                Value = u.Id.ToString()
            }).ToListAsync();
            OrderVM order = new()
            {
                ProductList = productList,
                Orders = new Orders()
            };
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderVM model)
        {
            if (ModelState.IsValid)
            {
                var seller = await GetSellerId();
                model.Orders.SellersId = seller.Id;
                model.Orders.Status = "unpaid";
                await db.Orders.AddAsync(model.Orders);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            Orders order = await db.Orders.Where(u => u.Id == id).FirstOrDefaultAsync();
            order.Status = "paid";
            db.Orders.Update(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
