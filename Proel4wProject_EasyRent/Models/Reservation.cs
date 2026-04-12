using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proel4wProject_EasyRent.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        public int UserId { get; set; }
        public int VehicleId { get; set; }

        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public int PassengerCapacity { get; set; }

        public DateTime PickupDate { get; set; }
        public TimeSpan PickupTime { get; set; }
        public DateTime ReturnDate { get; set; }
        public TimeSpan ReturnTime { get; set; }

        public string? UsageType { get; set; }
        public string? PickupAddress { get; set; }
        public string? DestinationAddress { get; set; }

        // Pricing Info
        public decimal TotalPartialFees { get; set; }
        public decimal SucceedingFee { get; set; }
        public decimal Downpayment { get; set; }
        public decimal FinalTotalCost { get; set; }

        // Payment Info
        public string? PaymentMethod { get; set; }
        public string? PaymentAccountName { get; set; }
        public string? PaymentReferenceNumber { get; set; }
        public decimal PaymentAmountSent { get; set; }

        // Admin Tracking
        public string Status { get; set; } = "Pending Verification";
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public Users? User { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }
    }
}
