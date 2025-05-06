using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismWebsite.Data;
using TourismWebsite.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Eklendi
using TourismWebsite.ViewModels; // ViewModel namespace'i eklendi
using Microsoft.AspNetCore.Mvc.Rendering;

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
        // GET: /Tours?destinationId=5
        public async Task<IActionResult> Index(int? destinationId)
        {
            IQueryable<Tour> toursQuery = _context.Tours.Include(t => t.Destination);

            if (destinationId.HasValue && destinationId.Value > 0)
            {
                toursQuery = toursQuery.Where(t => t.DestinationId == destinationId.Value);
                var destination = await _context.Destinations.FindAsync(destinationId.Value);
                ViewData["PageTitle"] = destination != null ? $"{destination.Name} Turları" : "Filtrelenmiş Turlar";
            }
            else
            {
                ViewData["PageTitle"] = "Tüm Turlar";
            }

            var toursSummary = await toursQuery
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
        
        // GET: Tours/Create
        // [Authorize(Roles = "Admin")] // Bu satır kaldırılacak veya yorumda kalacak
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                // Admin değilse giriş sayfasına yönlendir, belki bir returnUrl ile?
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", "Tours") });
            }

            // Destinasyonları dropdown için hazırla
            ViewBag.Destinations = new SelectList(await _context.Destinations.OrderBy(d => d.Name).ToListAsync(), "Id", "Name");
            return View(new CreateTourViewModel()); // Boş bir ViewModel ile formu başlat
        }

        // POST: Tours/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin")] // Bu satır kaldırılacak veya yorumda kalacak
        public async Task<IActionResult> Create(CreateTourViewModel model)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                return RedirectToAction("Login", "Account"); // Sadece login'e yönlendirme de yeterli olabilir
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Bitiş tarihi, başlangıç tarihinden önce olamaz.");
            }

            if (ModelState.IsValid)
            {
                var newTour = new Tour
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    DurationDays = model.DurationDays,
                    Itinerary = model.Itinerary,
                    ImageUrl = model.ImageUrl,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Capacity = model.Capacity,
                    DestinationId = model.DestinationId
                    // Destination navigation property'si otomatik olarak EF Core tarafından yönetilecektir.
                };

                _context.Tours.Add(newTour);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tur başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            // Model geçerli değilse, dropdownlist'i tekrar doldur ve formu göster
            ViewBag.Destinations = new SelectList(await _context.Destinations.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", model.DestinationId);
            return View(model);
        }

        // GET: Tours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Edit", "Tours", new { id }) });
            }

            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return NotFound();
            }

            var viewModel = new EditTourViewModel
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
                DestinationId = tour.DestinationId
            };

            ViewBag.Destinations = new SelectList(await _context.Destinations.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", tour.DestinationId);
            return View(viewModel);
        }

        // POST: Tours/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTourViewModel model)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.Id)
            {
                return NotFound(); // Gönderilen ID ile modeldeki ID uyuşmuyorsa
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Bitiş tarihi, başlangıç tarihinden önce olamaz.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tourToUpdate = await _context.Tours.FindAsync(model.Id);
                    if (tourToUpdate == null)
                    {
                        return NotFound(); // Güncellenecek tur bulunamadı
                    }

                    tourToUpdate.Name = model.Name;
                    tourToUpdate.Description = model.Description;
                    tourToUpdate.Price = model.Price;
                    tourToUpdate.DurationDays = model.DurationDays;
                    tourToUpdate.Itinerary = model.Itinerary;
                    tourToUpdate.ImageUrl = model.ImageUrl;
                    tourToUpdate.StartDate = model.StartDate;
                    tourToUpdate.EndDate = model.EndDate;
                    tourToUpdate.Capacity = model.Capacity;
                    tourToUpdate.DestinationId = model.DestinationId;

                    _context.Update(tourToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tur başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Eş zamanlılık çakışması durumunda (başka bir kullanıcı aynı anda kaydı değiştirdiyse)
                    // Bu durumu ele almak için özel bir mantık eklenebilir.
                    // Şimdilik, eğer tur artık yoksa NotFound döndürelim.
                    if (!TourExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Kayıt başka bir kullanıcı tarafından değiştirilmiş. Lütfen tekrar deneyin.");
                    }
                }
            }
            
            // Model geçerli değilse veya bir çakışma olduysa, dropdownlist'i tekrar doldur ve formu göster
            ViewBag.Destinations = new SelectList(await _context.Destinations.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", model.DestinationId);
            return View(model);
        }

        private bool TourExists(int id)
        {
            return _context.Tours.Any(e => e.Id == id);
        }

        // GET: Tours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Delete", "Tours", new { id }) });
            }

            if (id == null)
            {
                return NotFound();
            }

            var tour = await _context.Tours
                .Include(t => t.Destination) // Detayları göstermek için
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            
            if (tour == null)
            {
                return NotFound();
            }

            // Onay sayfasında göstermek için bir ViewModel kullanabiliriz veya doğrudan Tour modelini gönderebiliriz.
            // Şimdilik TourDetailViewModel kullanalım, çünkü zaten var ve gerekli bilgileri içeriyor.
            var viewModel = new TourDetailViewModel
            {
                Id = tour.Id,
                Name = tour.Name,
                DestinationName = tour.Destination?.Name,
                StartDate = tour.StartDate,
                EndDate = tour.EndDate
                // Diğer gerekli alanlar eklenebilir
            };

            return View(viewModel); // Delete.cshtml view'ına modeli gönder
        }

        // POST: Tours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "True")
            {
                return RedirectToAction("Login", "Account");
            }

            var tourToDelete = await _context.Tours.FindAsync(id);
            if (tourToDelete == null)
            {
                TempData["ErrorMessage"] = "Silinecek tur bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Tours.Remove(tourToDelete);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tur başarıyla silindi.";
            }
            catch (DbUpdateException /* ex */)
            {
                // Silme sırasında bir veritabanı hatası oluşursa (örn: ilişkili kayıtlar nedeniyle silinemiyorsa)
                TempData["ErrorMessage"] = "Tur silinirken bir hata oluştu. İlişkili kayıtları olabilir.";
                // Hata detayını loglayabiliriz: _logger.LogError(ex, "Error deleting tour {TourId}", id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 