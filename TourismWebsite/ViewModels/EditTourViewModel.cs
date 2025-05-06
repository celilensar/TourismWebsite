using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsite.ViewModels
{
    public class EditTourViewModel
    {
        public int Id { get; set; } // Düzenlenecek turun ID'si

        [Required(ErrorMessage = "Tur adı gereklidir.")]
        [StringLength(150, ErrorMessage = "Tur adı en fazla 150 karakter olabilir.")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Fiyat alanı gereklidir.")]
        // [Column(TypeName = "decimal(18, 2)")] // Bu attribute modelde kalmalı, ViewModel için Range yeterli.
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Süre alanı gereklidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "Süre en az 1 gün olmalıdır.")]
        [Display(Name = "Süre (Gün)")]
        public int DurationDays { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Itinerary { get; set; }

        [Display(Name = "Resim URL")]
        [DataType(DataType.ImageUrl)]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi gereklidir.")]
        [DataType(DataType.Date)]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi gereklidir.")]
        [DataType(DataType.Date)]
        [Display(Name = "Bitiş Tarihi")]
        // TODO: Bitiş tarihinin başlangıç tarihinden sonra olduğuna dair custom validation eklenebilir.
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Kontenjan gereklidir.")]
        [Range(1, int.MaxValue, ErrorMessage = "Kontenjan en az 1 olmalıdır.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Lütfen bir destinasyon seçin.")]
        [Display(Name = "Destinasyon")]
        public int DestinationId { get; set; }
    }
} 