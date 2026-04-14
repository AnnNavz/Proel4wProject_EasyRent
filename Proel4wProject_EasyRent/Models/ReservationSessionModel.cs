using System.ComponentModel.DataAnnotations;

namespace Proel4wProject_EasyRent.Models
{
    public class ReservationSessionModel
    {
        // IDs
        [Required(ErrorMessage = "User ID is required.")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vehicle is required.")]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        [StringLength(100, ErrorMessage = "Vehicle name cannot exceed 100 characters.")]
        [Display(Name = "Vehicle Name")]
        public string? VehicleName { get; set; }

        [StringLength(50, ErrorMessage = "Vehicle type cannot exceed 50 characters.")]
        [Display(Name = "Vehicle Type")]
        public string? VehicleType { get; set; }

        // Step 2: Renter Details
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }

        [Range(1, 50, ErrorMessage = "Passenger capacity must be between 1 and 50.")]
        [Display(Name = "Passenger Capacity")]
        public int PassengerCapacity { get; set; }
        
        [Required(ErrorMessage = "Pickup date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Pickup Date")]
        public DateTime PickupDate { get; set; }

        [Required(ErrorMessage = "Pickup time is required.")]
        [DataType(DataType.Time)]
        [Display(Name = "Pickup Time")]
        public TimeSpan PickupTime { get; set; }

        [Required(ErrorMessage = "Return date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; }

        [Required(ErrorMessage = "Return time is required.")]
        [DataType(DataType.Time)]
        [Display(Name = "Return Time")]
        public TimeSpan ReturnTime { get; set; }
        
        [Required(ErrorMessage = "Usage type is required.")]
        [StringLength(50, ErrorMessage = "Usage type cannot exceed 50 characters.")]
        [Display(Name = "Usage Type")]
        public string? UsageType { get; set; }

        [Required(ErrorMessage = "Pickup address is required.")]
        [StringLength(300, ErrorMessage = "Pickup address cannot exceed 300 characters.")]
        [Display(Name = "Pickup Address")]
        public string? PickupAddress { get; set; }

        [Required(ErrorMessage = "Destination address is required.")]
        [StringLength(300, ErrorMessage = "Destination address cannot exceed 300 characters.")]
        [Display(Name = "Destination Address")]
        public string? DestinationAddress { get; set; }

        // Math/Computed fields for display (Calculated in Step 2 -> 3)
        [Range(0, 9999999.99, ErrorMessage = "Base price must be a valid amount.")]
        [Display(Name = "Base Price")]
        public decimal BasePrice { get; set; }

        [StringLength(50, ErrorMessage = "Price per label cannot exceed 50 characters.")]
        [Display(Name = "Price Per")]
        public string? PricePer { get; set; } // "per hour" or "per 3 hours" etc

        [Range(0, 9999999.99, ErrorMessage = "Total partial fees must be a valid amount.")]
        [Display(Name = "Total Partial Fees")]
        public decimal TotalPartialFees { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Succeeding fee must be a valid amount.")]
        [Display(Name = "Succeeding Fee")]
        public decimal SucceedingFee { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Downpayment must be a valid amount.")]
        [Display(Name = "Downpayment")]
        public decimal Downpayment { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Total fee must be a valid amount.")]
        [Display(Name = "Total Fee To Be Paid")]
        public decimal TotalFeeToBePaid { get; set; } // The final required payment for this step

        [Range(0, 9999999.99, ErrorMessage = "Final total cost must be a valid amount.")]
        [Display(Name = "Final Total Cost")]
        public decimal FinalTotalCost { get; set; } // The actual full cost

        [StringLength(200, ErrorMessage = "Time display cannot exceed 200 characters.")]
        [Display(Name = "Time Accumulated")]
        public string? TimeAccumulatedDisplay { get; set; }

        // Step 3: Payment Type
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
        [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; } // "On-site" or "GCash" or "BDO"

        // Step 4: Payment Confirmation
        [StringLength(150, ErrorMessage = "Account name cannot exceed 150 characters.")]
        [Display(Name = "Account Name")]
        public string? AccountName { get; set; }

        [StringLength(100, ErrorMessage = "Reference number cannot exceed 100 characters.")]
        [Display(Name = "Reference Number")]
        public string? ReferenceNumber { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Amount sent must be a valid amount.")]
        [Display(Name = "Amount Sent")]
        public decimal AmountSent { get; set; }
    }
}
