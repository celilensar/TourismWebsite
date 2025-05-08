using System.ComponentModel.DataAnnotations;

namespace TourismWebsite.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TourId { get; set; }
        public int? CardId { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        
        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Paid, Cancelled

        // Navigation properties
        public User User { get; set; } = null!;
        public Tour Tour { get; set; } = null!;
        public Card? Card { get; set; }
    }
} 