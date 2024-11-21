using CoolShopFlowers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CoolShopFlowers.Controllers
{
    public class FlowersController : Controller
    {
        private readonly DBContext _context;

        public FlowersController(DBContext context)
        {
            _context = context;
        }

        // GET: Flowers
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Flowers.Include(f => f.Category);
            return View(await dBContext.ToListAsync());
        }

        // GET: Flowers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Flowers == null)
            {
                return NotFound();
            }

            var flower = await _context.Flowers
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.FlowerId == id);
            if (flower == null)
            {
                return NotFound();
            }

            return View(flower);
        }

        //// GET: Flowers/Create
        //public IActionResult Create()
        //{
        //    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
        //    return View();
        //}

        // GET: Flowers/Create
        public IActionResult Create()
        {
            // Change the SelectList to use the Category Name, not the CategoryId
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");

            // Додаємо список для FlowerType
            ViewData["FlowerType"] = new SelectList(Enum.GetValues(typeof(FlowerType)).Cast<FlowerType>().Select(f => new
            {
                Value = f,
                Text = f.ToString()
            }), "Value", "Text");

            return View();
        }

        // POST: Flowers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FlowerId,Name,Price,Description,CategoryId,FlowerType")] Flower flower)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Відправка даних про категорії та типи квіток назад до View
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", flower.CategoryId);
            ViewData["FlowerType"] = new SelectList(Enum.GetValues(typeof(FlowerType)).Cast<FlowerType>().Select(f => new
            {
                Value = f,
                Text = f.ToString()
            }), "Value", "Text", flower.FlowerType);

            return View(flower);
        }

        // GET: Flowers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Flowers == null)
            {
                return NotFound();
            }

            var flower = await _context.Flowers.FindAsync(id);
            if (flower == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", flower.CategoryId);
            return View(flower);
        }

        // POST: Flowers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FlowerId,Name,Price,Description,CategoryId")] Flower flower)
        {
            if (id != flower.FlowerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlowerExists(flower.FlowerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", flower.CategoryId);
            return View(flower);
        }

        // GET: Flowers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Flowers == null)
            {
                return NotFound();
            }

            var flower = await _context.Flowers
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.FlowerId == id);
            if (flower == null)
            {
                return NotFound();
            }

            return View(flower);
        }

        // POST: Flowers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Flowers == null)
            {
                return Problem("Entity set 'DBContext.Flowers'  is null.");
            }
            var flower = await _context.Flowers.FindAsync(id);
            if (flower != null)
            {
                _context.Flowers.Remove(flower);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlowerExists(int id)
        {
          return (_context.Flowers?.Any(e => e.FlowerId == id)).GetValueOrDefault();
        }


    }
}
