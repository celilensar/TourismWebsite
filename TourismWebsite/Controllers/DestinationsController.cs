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
            // Admin kontrolü
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                // Admin değilse ana sayfaya yönlendir (veya bir hata mesajı göster)
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }
            
            // Admin ise boş formu göster
            return View();
        }

        // POST: Destinations/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageUrl")] Destination destination) // Bind ile sadece beklenen alanları al
        {
            // Admin kontrolü
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                TempData["ErrorMessage"] = "Bu işlem için yetkiniz yok.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid) // Model validasyonları geçerli mi?
            {
                _context.Add(destination);
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet
                
                TempData["SuccessMessage"] = "Destinasyon başarıyla eklendi."; // Başarı mesajı
                return RedirectToAction(nameof(Index)); // Index aksiyonuna yönlendir
            }
            
            // Model geçerli değilse, formu hatalarla birlikte tekrar göster
            return View(destination);
        }

        // Buraya Edit, Details, Delete aksiyonları eklenecek
    }
} 