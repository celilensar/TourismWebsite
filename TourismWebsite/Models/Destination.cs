using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourismWebsite.Models
{
    public class Destination
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Destinasyon adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Destinasyon adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string? ImageUrl { get; set; } // Nullable string

        // Navigation Property
        public List<Tour>? Tours { get; set; }
    }
} 