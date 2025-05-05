using Microsoft.AspNetCore.Mvc;
using TourismWebsite.Data;
using TourismWebsite.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Session için eklendi

namespace TourismWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly TourismContext _context;

        public AccountController(TourismContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarını önlemek için
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Email zaten var mı kontrol et
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Bu e-posta adresi zaten kullanılıyor.");
                    return View(model);
                }

                var user = new User
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    Password = model.Password, // Şifre düz metin olarak kaydediliyor
                    Age = model.Age,
                    Gender = model.Gender,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    IsAdmin = false // Varsayılan olarak admin değil
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Kayıt başarılı olduktan sonra login sayfasına yönlendir
                // İsteğe bağlı olarak TempData ile başarı mesajı gösterilebilir
                TempData["SuccessMessage"] = "Başarıyla kayıt oldunuz. Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login", "Account");
            }

            // Model geçerli değilse formu tekrar göster
            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Kayıt sonrası yönlendirmeden gelen başarı mesajını alıp ViewData'ya aktaralım
            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Yönlendirme için ReturnUrl'i tekrar View'a gönder
            if (ModelState.IsValid)
            {
                // Kullanıcıyı e-posta adresine göre bul
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                // Kullanıcı var mı ve şifre doğru mu kontrol et (Düz metin karşılaştırması)
                if (user != null && user.Password == model.Password) 
                {
                    // Başarılı giriş: Session başlat
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetInt32("UserId", user.Id); // Kullanıcı ID'sini de saklayabiliriz
                    HttpContext.Session.SetString("UserName", user.Name); // Kullanıcı Adını da saklayabiliriz
                    HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString()); // Admin durumunu string olarak sakla

                    // Eğer returnUrl geçerli ise oraya, değilse ana sayfaya yönlendir
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
                    }
                }
                else
                {
                    // Başarısız giriş: Hata mesajı ekle
                    ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
                    return View(model);
                }
            }

            // Model geçerli değilse formu tekrar göster
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Session'ı temizle
            HttpContext.Session.Clear();

            // Ana sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }

    }
} 