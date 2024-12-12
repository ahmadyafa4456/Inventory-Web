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
    public class ProductController : Controller
    {
        private readonly string pathlocation = $"{Directory.GetCurrentDirectory()}\\wwwroot\\img\\product";
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<AppUser> userManager;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager)
        {
            this.db = db;
            this.webHostEnvironment = webHostEnvironment;
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
            List<Products> product = await db.Product.Include(u => u.Category).Where(u => u.SellerId == seller.Id).ToListAsync();
            if (product != null)
            {
                return View(product);
            }
            return View();
        }

        public IActionResult Create()
        {
            ProductVM product = new ProductVM
            {
                CategoryList = db.Category.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                product = new Products()
            };
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM model, IFormFile? file)
        {
            ProductVM product = new ProductVM
            {
                CategoryList = db.Category.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                product = model.product
            };
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    if (!Directory.Exists(pathlocation))
                    {
                        Directory.CreateDirectory(pathlocation);
                    }
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(pathlocation, filename);
                    using (var fileStream = new FileStream(productPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    model.product.url = @"\img\product\" + filename;
                }
                else
                {
                    ModelState.AddModelError("product.url", "Image must be required");
                }
                var seller = await GetSellerId();
                model.product.SellerId = seller.Id;
                model.product.status = "in stock";
                await db.Product.AddAsync(model.product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public IActionResult Edit(int? id)
        {
            ProductVM product = new ProductVM
            {
                CategoryList = db.Category.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                product = db.Product.Where(u => u.Id == id).FirstOrDefault()
            };
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductVM model, IFormFile? file)
        {
            if (model.product.url == null)
            {
                ModelState.AddModelError("product.url", "Image must be required");
            }
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(pathlocation, filename);
                    if (!string.IsNullOrEmpty(model.product.url))
                    {
                        string wwwRootPath = webHostEnvironment.WebRootPath;
                        var oldImage = Path.Combine(wwwRootPath, model.product.url.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }
                    using (var fileStream = new FileStream(productPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    model.product.url = @"\img\product\" + filename;
                }
                db.Product.Update(model.product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            Products product = await db.Product.Where(u => u.Id == id).FirstOrDefaultAsync();
            Orders order = await db.Orders.Where(u => u.ProductsId == product.Id).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(product.url))
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                var oldImage = Path.Combine(wwwRootPath, product.url.TrimStart('\\'));
                if (System.IO.File.Exists(oldImage))
                {
                    System.IO.File.Delete(oldImage);
                }
            }
            db.Orders.Remove(order);
            db.Product.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
