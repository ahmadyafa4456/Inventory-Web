using Inventory_Web.Data;
using Inventory_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext db;
        public CategoryController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> category = await db.Category.ToListAsync();
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                await db.Category.AddAsync(model);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            Category category = db.Category.Where(u => u.Id == id).FirstOrDefault();
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Category.Update(category);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int? id)
        {
            Category category = db.Category.Where(u => u.Id == id).FirstOrDefault();
            if(category == null)
            {
                return NotFound();
            }
            db.Category.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
