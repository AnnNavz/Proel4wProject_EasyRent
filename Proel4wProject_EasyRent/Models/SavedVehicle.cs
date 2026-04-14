using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proel4wProject_EasyRent.Models
{
    public class SavedVehicle
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User is required.")]
        [Display(Name = "User")]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Vehicle is required.")]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Saved")]
        public DateTime SavedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public Users? User { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }
    }
}
