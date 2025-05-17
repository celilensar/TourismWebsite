using Microsoft.AspNetCore.Mvc;
using TourismWebsite.Data;
using TourismWebsite.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Session için eklendi

namespace TourismWebsite.Controllers
{
    public class DestinationsController : Controller
    {
        private readonly TourismContext _context;

        public DestinationsController(TourismContext context)
        {
            _context = context;
        }

        // GET: Destinations
        public async Task<IActionResult> Index()
        {
            var destinations = await _context.Destinations.ToListAsync();
            return View(destinations); // Listeyi View'a gönder
        }

        // GET: Destinations/Create
        public IActionResult Create()
        {
            // Önce session kontrolü
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData.Clear(); // TempData'yı temizle
                return RedirectToAction("Login", "Account");
            }

            // Sonra admin kontrolü
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        // POST: Destinations/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageUrl")] Destination destination)
        {
            // Önce session kontrolü
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData.Clear(); // TempData'yı temizle
                return RedirectToAction("Login", "Account");
            }

            // Sonra admin kontrolü
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Add(destination);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Destinasyon başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(destination);
        }

        // GET: Destinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Önce session kontrolü
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData.Clear(); // TempData'yı temizle
                return RedirectToAction("Login", "Account");
            }

            // Sonra admin kontrolü
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                return NotFound();
            }
            return View(destination);
        }

        // POST: Destinations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageUrl")] Destination destination)
        {
            // Önce session kontrolü
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData.Clear(); // TempData'yı temizle
                return RedirectToAction("Login", "Account");
            }

            // Sonra admin kontrolü
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            if (id != destination.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(destination);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Destinasyon başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DestinationExists(destination.Id))
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
            return View(destination);
        }

        // GET: Destinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Session ve admin kontrolü (gerekirse, genellikle detaylar herkes tarafından görülebilir
            // ancak bu projede admin yetkisi gerektiren bir yapı kurulmuş)
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData.Clear();
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                // Ana sayfaya veya destinasyon listesine yönlendirme daha uygun olabilir.
                // Şimdilik Home/Index varsayalım, gerekirse Index'e yönlendirilebilir.
                return RedirectToAction("Index", "Home"); 
            }

            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        // GET: Destinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Önce session kontrolü
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData.Clear(); // TempData'yı temizle
                return RedirectToAction("Login", "Account");
            }

            // Sonra admin kontrolü
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (destination == null)
            {
                return NotFound();
            }

            return View(destination);
        }

        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Önce session kontrolü
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                TempData.Clear(); // TempData'yı temizle
                return RedirectToAction("Login", "Account");
            }

            // Sonra admin kontrolü
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            var destination = await _context.Destinations.FindAsync(id);
            if (destination != null)
            {
                _context.Destinations.Remove(destination);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Destinasyon başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Silinecek destinasyon bulunamadı.";
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.Id == id);
        }

        // Buraya Edit, Details, Delete aksiyonları eklenecek
    }
} 