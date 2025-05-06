namespace TourismWebsite.ViewModels
{
    public class TourSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? DestinationName { get; set; } // Turun bağlı olduğu destinasyonun adı
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        // İsteğe bağlı olarak ek özellikler eklenebilir, örneğin kısa bir açıklama
    }
} 