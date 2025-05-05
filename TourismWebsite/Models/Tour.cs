using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsite.Models
{
    public class Tour
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tur adı gereklidir.")]
        [StringLength(150, ErrorMessage = "Tur adı en fazla 150 karakter olabilir.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Fiyat alanı gereklidir.")]
        [Column(TypeName = "decimal(18, 2)")] // Veritabanında para birimi için uygun tip
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Süre en az 1 gün olmalıdır.")]
        public int DurationDays { get; set; }

        public string Itinerary { get; set; }

        public string? ImageUrl { get; set; } // Nullable string

        // Foreign Key
        [Required(ErrorMessage = "Lütfen bir destinasyon seçin.")]
        public int DestinationId { get; set; }

        // Navigation Property
        [ForeignKey("DestinationId")]
        public Destination Destination { get; set; }
    }
} 