namespace TourismWebsite.ViewModels
{
    public class TourDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string? Itinerary { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public string? DestinationName { get; set; } // Turun bağlı olduğu destinasyonun adı

        // Görünüme özel ek özellikler veya formatlanmış veriler buraya eklenebilir
        // Örneğin:
        // public string FormattedPrice => Price.ToString("C");
        // public string DateRange => $"{StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}";
    }
} 