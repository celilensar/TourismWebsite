using System.ComponentModel.DataAnnotations;

namespace TourismWebsite.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "E-posta alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).+$", ErrorMessage = "Şifre en az bir büyük ve bir küçük harf içermelidir.")]
        public string Password { get; set; } // Hash'lenmeyecek

        public bool IsAdmin { get; set; }
    }
} 