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
            // Sadece kayıt başarılı mesajını göster
            if (TempData["SuccessMessage"] != null && TempData["SuccessMessage"].ToString().Contains("kayıt oldunuz"))
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

        public IActionResult Profile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(User model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.Age = model.Age;
            user.Gender = model.Gender;

            _context.SaveChanges();

            HttpContext.Session.SetString("UserName", $"{user.Name} {user.Surname}");

            TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
            return RedirectToAction("Profile");
        }
    }
} 