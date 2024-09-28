using Inventory_Web.Data;
using Inventory_Web.Models;
using Inventory_Web.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory_Web.Areas.Seller.Controllers;

namespace Inventory_Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class SellersController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public SellersController(UserManager<AppUser> userManager, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Sellers> seller = db.Sellers.Include(a => a.Address).ToList();
            return View(seller);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SellerVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Name = model.Name,
                    UserName = model.Name,
                    Email = model.Email
                };
                var createUser = await userManager.CreateAsync(user, model.Password!);
                if (createUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Seller");
                    var userId = await userManager.FindByNameAsync(user.UserName);
                    if (userId == null)
                    {
                        return NotFound();
                    }
                    Sellers seller = new Sellers
                    {
                        Name = model.Name,
                        UserId = userId.Id
                    };
                    await db.Sellers.AddAsync(seller);
                    await db.SaveChangesAsync();
                    var sellerId = db.Sellers.Where(u => u.Name == model.Name).FirstOrDefault();
                    if (sellerId == null)
                    {
                        return NotFound();
                    }
                    Address address = new Address
                    {
                        City = model.City,
                        Street = model.Street,
                        SellerId = sellerId.Id
                    };
                    await db.Address.AddAsync(address);
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int? id)
        {
            var seller = db.Sellers.Include(a => a.Address).Include(u => u.User).Where(s => s.Id == id).FirstOrDefault();
            return View(seller);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Sellers model)
        {
            if (ModelState.IsValid)
            {
                var seller = db.Sellers.Include(u => u.User).Include(u => u.Address).FirstOrDefault(u => u.Id == model.Id);
                if (seller == null)
                {
                    return NotFound();
                }
                seller.Name = model.Name;
                if(seller.User != null)
                {
                    seller.User.Email = model.User.Email;
                }
                if(seller.Address != null)
                {
                    seller.Address.City = model.Address.City;
                    seller.Address.Street = model.Address.Street;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var seller = await db.Sellers.Where(u => u.Id == id).FirstOrDefaultAsync();
            var user = await db.User.Where(u => u.Id == seller.UserId).FirstOrDefaultAsync();
            var address = await db.Address.Where(u => u.SellerId == id).FirstOrDefaultAsync();
            var order = await db.Orders.Where(u => u.SellersId == id).FirstOrDefaultAsync();
            if(order != null)
            {
                db.Orders.Remove(order);
            }
            await DeleteImageProduct(seller.Id);
            db.Address.Remove(address);
            db.User.Remove(user);
            db.Sellers.Remove(seller);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Product(int? id)
        {
            List<Products> product = db.Product.Where(u => u.SellerId == id).Include(u => u.Category).ToList();
            return View(product);
        }

        private async Task DeleteImageProduct(int sellerId)
        {
            string wwwRootPath = webHostEnvironment.WebRootPath;
            List<Products> product = await db.Product.Where(u => u.SellerId == sellerId).ToListAsync();
            if (product != null) 
            {
                foreach (var p in product)
                {
                    var oldImage = Path.Combine(wwwRootPath, p.url.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImage))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                }
            }
        }
    }
}
