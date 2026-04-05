using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proel4wProject_EasyRent.Models
{
	public class Vehicle
	{
		[Key]
		public int VehicleId { get; set; }

		[Required]
		public string ModelName { get; set; }

		[Required]
		[Column(TypeName = "decimal(18, 2)")]
		public decimal StartingPrice { get; set; }

		public virtual ICollection<VehicleBenefit> Benefits { get; set; } = new List<VehicleBenefit>();
	}
}
