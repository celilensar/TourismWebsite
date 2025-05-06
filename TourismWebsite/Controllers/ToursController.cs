using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismWebsite.Data;
using TourismWebsite.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Eklendi
using TourismWebsite.ViewModels; // ViewModel namespace'i eklendi

namespace TourismWebsite.Controllers
{
    public class ToursController : Controller
    {
        private readonly TourismContext _context;

        public ToursController(TourismContext context)
        {
            _context = context;
        }

        // İleride buraya Index, Details, Create, Edit, Delete action'ları eklenecek
        // Örneğin:
        // GET: /Tours
        public async Task<IActionResult> Index()
        {
            var toursSummary = await _context.Tours
                .Include(t => t.Destination) // DestinationName için gerekli
                .Select(t => new TourSummaryViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    DestinationName = t.Destination != null ? t.Destination.Name : "Belirtilmemiş",
                    Price = t.Price,
                    ImageUrl = t.ImageUrl,
                    StartDate = t.StartDate
                }).ToListAsync();
            
            return View(toursSummary);
        }

        // GET: /Tours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.Destination) // DestinationName için gerekli
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (tour == null)
            {
                return NotFound();
            }

            var tourDetailViewModel = new TourDetailViewModel
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Price = tour.Price,
                DurationDays = tour.DurationDays,
                Itinerary = tour.Itinerary,
                ImageUrl = tour.ImageUrl,
                StartDate = tour.StartDate,
                EndDate = tour.EndDate,
                Capacity = tour.Capacity,
                DestinationName = tour.Destination != null ? tour.Destination.Name : "Belirtilmemiş"
            };

            return View(tourDetailViewModel);
        }
        
        // Daha sonra Admin CRUD işlemleri eklenecek
        // GET: Tours/Create
        // [Authorize(Roles = "Admin")]
        // public IActionResult Create()
        // {
        //     // Gerekli verileri (örn: Destinasyon listesi) ViewBag veya ViewModel ile view'a taşı
        //     return View();
        // }

    }
} 