using System.ComponentModel.DataAnnotations;

namespace TourismWebsite.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "İsim alanı gereklidir.")]
        [StringLength(50)]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyisim alanı gereklidir.")]
        [StringLength(50)]
        [Display(Name = "Soyisim")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Yaş")]
        [Range(0, 120, ErrorMessage = "Geçerli bir yaş giriniz.")]
        public int? Age { get; set; }

        [Display(Name = "Cinsiyet")]
        public string? Gender { get; set; }

        [Display(Name = "Telefon Numarası")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Adres")]
        public string? Address { get; set; }
    }
} 