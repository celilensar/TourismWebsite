using Microsoft.AspNetCore.Mvc;
using TourismWebsite.Data;
using TourismWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace TourismWebsite.Controllers
{
    public class CardController : Controller
    {
        private readonly TourismContext _context;

        public CardController(TourismContext context)
        {
            _context = context;
        }

        // GET: /Card/MyCards
        public async Task<IActionResult> MyCards()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            var cards = await _context.Cards
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cards);
        }

        // GET: /Card/Add
        public IActionResult Add()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: /Card/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Card card)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                card.UserId = userId.Value;
                _context.Cards.Add(card);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kart bilgileriniz başarıyla kaydedildi.";
                return RedirectToAction(nameof(MyCards));
            }

            return View(card);
        }

        // POST: /Card/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "True";

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (isAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (card == null)
            {
                return NotFound();
            }

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Kart başarıyla silindi.";
            return RedirectToAction(nameof(MyCards));
        }
    }
} 