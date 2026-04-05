using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proel4wProject_EasyRent.Models
{
	public class VehicleBenefit
	{
		[Key]
		public int BenefitId { get; set; }

		[Required]
		[StringLength(100)]
		public string Description { get; set; } // e.g., "7 seats capacity"

		// Foreign Key to Vehicle
		[Required]
		public int VehicleId { get; set; }

		// Navigation property
		[ForeignKey("VehicleId")]
		public virtual Vehicle Vehicle { get; set; }
	}
}
