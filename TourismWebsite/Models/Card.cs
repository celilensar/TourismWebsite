using System.ComponentModel.DataAnnotations;

namespace TourismWebsite.Models
{
    public class Card
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kart numarası gereklidir.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Kart numarası 16 haneli olmalıdır.")]
        public string CardNumber { get; set; } = null!;

        [Required(ErrorMessage = "Kart sahibi adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Kart sahibi adı en fazla 100 karakter olabilir.")]
        public string CardHolderName { get; set; } = null!;

        [Required(ErrorMessage = "Son kullanma tarihi gereklidir.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Son kullanma tarihi MM/YY formatında olmalıdır.")]
        public string ExpiryDate { get; set; } = null!;

        [Required(ErrorMessage = "CVV gereklidir.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV 3 haneli olmalıdır.")]
        public string CVV { get; set; } = null!;

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User? User { get; set; }
    }
} 