using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proel4wProject_EasyRent.Models
{
	public class VehicleImage
	{
		[Key]
		public int ImageId { get; set; }

		[Required]
		public int VehicleId { get; set; }

		[Required]
		[StringLength(255)]
		public string ImagePath { get; set; }

		[ForeignKey("VehicleId")]
		public virtual Vehicle Vehicle { get; set; }
	}
}
