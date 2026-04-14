using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proel4wProject_EasyRent.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required(ErrorMessage = "User is required.")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vehicle is required.")]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
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

        [StringLength(50, ErrorMessage = "Usage type cannot exceed 50 characters.")]
        [Display(Name = "Usage Type")]
        public string? UsageType { get; set; }

        [StringLength(300, ErrorMessage = "Pickup address cannot exceed 300 characters.")]
        [Display(Name = "Pickup Address")]
        public string? PickupAddress { get; set; }

        [StringLength(300, ErrorMessage = "Destination address cannot exceed 300 characters.")]
        [Display(Name = "Destination Address")]
        public string? DestinationAddress { get; set; }

        // Pricing Info
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, 9999999.99, ErrorMessage = "Total partial fees must be a valid amount.")]
        [Display(Name = "Total Partial Fees")]
        public decimal TotalPartialFees { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, 9999999.99, ErrorMessage = "Succeeding fee must be a valid amount.")]
        [Display(Name = "Succeeding Fee")]
        public decimal SucceedingFee { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, 9999999.99, ErrorMessage = "Downpayment must be a valid amount.")]
        [Display(Name = "Downpayment")]
        public decimal Downpayment { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, 9999999.99, ErrorMessage = "Final total cost must be a valid amount.")]
        [Display(Name = "Final Total Cost")]
        public decimal FinalTotalCost { get; set; }

        // Payment Info
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
        [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; }

        [StringLength(150, ErrorMessage = "Account name cannot exceed 150 characters.")]
        [Display(Name = "Payment Account Name")]
        public string? PaymentAccountName { get; set; }

        [StringLength(100, ErrorMessage = "Reference number cannot exceed 100 characters.")]
        [Display(Name = "Reference Number")]
        public string? PaymentReferenceNumber { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, 9999999.99, ErrorMessage = "Amount sent must be a valid amount.")]
        [Display(Name = "Amount Sent")]
        public decimal PaymentAmountSent { get; set; }

        // Admin Tracking
        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending Verification";

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public Users? User { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }
    }
}
