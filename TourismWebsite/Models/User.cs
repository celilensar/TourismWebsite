using System.ComponentModel.DataAnnotations;

namespace TourismWebsite.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim alanı gereklidir.")]
        [StringLength(50, ErrorMessage = "İsim en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyisim alanı gereklidir.")]
        [StringLength(50, ErrorMessage = "Soyisim en fazla 50 karakter olabilir.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        // Şifre validasyonlarını RegisterViewModel'da yapacağız, burada sade bırakabiliriz.
        public string Password { get; set; } // Düz metin olarak saklanacak

        public int? Age { get; set; } // İsteğe bağlı
        public string? Gender { get; set; } // İsteğe bağlı (örn: "Erkek", "Kadın", "Belirtmek İstemiyor")
        
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string? PhoneNumber { get; set; } // İsteğe bağlı

        public string? Address { get; set; } // İsteğe bağlı

        public bool IsAdmin { get; set; } // Yönetici mi?
    }
} 