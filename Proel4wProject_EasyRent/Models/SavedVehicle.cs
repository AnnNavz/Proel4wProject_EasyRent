using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proel4wProject_EasyRent.Models
{
    public class SavedVehicle
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        
        public int VehicleId { get; set; }

        public DateTime SavedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public Users? User { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }
    }
}
