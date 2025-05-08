using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismWebsite.Data;
using TourismWebsite.Models;
using TourismWebsite.ViewModels;

namespace TourismWebsite.Controllers
{
    public class BookingController : Controller
    {
        private readonly TourismContext _context;

        public BookingController(TourismContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int tourId, int numberOfPeople)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (isAdmin)
            {
                TempData["Error"] = "Admin kullanıcılar rezervasyon yapamaz.";
                return RedirectToAction("Details", "Tours", new { id = tourId });
            }

            var tour = await _context.Tours.FindAsync(tourId);
            if (tour == null)
            {
                return NotFound();
            }

            // Kontenjan kontrolü
            var existingBookings = await _context.Bookings
                .Where(b => b.TourId == tourId && (b.Status == "Approved" || b.Status == "Paid"))
                .SumAsync(b => b.NumberOfPeople);

            if (existingBookings + numberOfPeople > tour.Capacity)
            {
                TempData["Error"] = "Üzgünüz, seçtiğiniz kişi sayısı için yeterli kontenjan bulunmamaktadır.";
                return RedirectToAction("Details", "Tours", new { id = tourId });
            }

            var booking = new Booking
            {
                UserId = userId.Value,
                TourId = tourId,
                NumberOfPeople = numberOfPeople,
                TotalPrice = tour.Price * numberOfPeople,
                BookingDate = DateTime.Now,
                Status = "Pending"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezervasyonunuz başarıyla oluşturuldu. Admin onayı beklemektedir.";
            return RedirectToAction("MyBookings");
        }

        public async Task<IActionResult> MyBookings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (isAdmin)
            {
                return RedirectToAction("AllBookings");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.Card)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            ViewBag.UserCards = await _context.Cards
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> AllBookings()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";
            if (!isAdmin)
            {
                return RedirectToAction("MyBookings");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.User)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";
            if (!isAdmin)
            {
                return RedirectToAction("MyBookings");
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "Approved";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezervasyon başarıyla onaylandı.";
            return RedirectToAction("AllBookings");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";
            if (!isAdmin)
            {
                return RedirectToAction("MyBookings");
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezervasyon başarıyla reddedildi.";
            return RedirectToAction("AllBookings");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status == "Paid")
            {
                TempData["Error"] = "Ödenmiş rezervasyonlar iptal edilemez.";
                return RedirectToAction("MyBookings");
            }

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezervasyonunuz başarıyla iptal edildi.";
            return RedirectToAction("MyBookings");
        }

        [HttpPost]
        public async Task<IActionResult> Pay(int id, int cardId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status != "Approved")
            {
                TempData["Error"] = "Sadece onaylanmış rezervasyonlar için ödeme yapılabilir.";
                return RedirectToAction("MyBookings");
            }

            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.Id == cardId && c.UserId == userId);

            if (card == null)
            {
                TempData["Error"] = "Geçersiz kart seçimi.";
                return RedirectToAction("MyBookings");
            }

            booking.Status = "Paid";
            booking.CardId = cardId;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ödemeniz başarıyla gerçekleştirildi.";
            return RedirectToAction("MyBookings");
        }
    }
} 