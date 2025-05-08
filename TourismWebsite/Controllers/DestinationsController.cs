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

        // Buraya Edit, Details, Delete aksiyonları eklenecek
    }
} 