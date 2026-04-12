using System.ComponentModel.DataAnnotations;

namespace Proel4wProject_EasyRent.Models
{
    public class ReservationSessionModel
    {
        // IDs
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? VehicleType { get; set; }

        // Step 2: Renter Details
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

        // Math/Computed fields for display (Calculated in Step 2 -> 3)
        public decimal BasePrice { get; set; }
        public string? PricePer { get; set; } // "per hour" or "per 3 hours" etc
        public decimal TotalPartialFees { get; set; }
        public decimal SucceedingFee { get; set; }
        public decimal Downpayment { get; set; }
        public decimal TotalFeeToBePaid { get; set; } // The final required payment for this step
        public decimal FinalTotalCost { get; set; } // The actual full cost

        public string? TimeAccumulatedDisplay { get; set; }

        // Step 3: Payment Type
        public string? PaymentMethod { get; set; } // "On-site" or "GCash" or "BDO"

        // Step 4: Payment Confirmation
        public string? AccountName { get; set; }
        public string? ReferenceNumber { get; set; }
        public decimal AmountSent { get; set; }
    }
}
