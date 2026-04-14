using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proel4wProject_EasyRent.Models
{
	public class Vehicle
	{
		[Key]
		public int VehicleId { get; set; }

		[Required(ErrorMessage = "Model name is required.")]
		[StringLength(100, ErrorMessage = "Model name cannot exceed 100 characters.")]
		[Display(Name = "Model Name")]
		public string ModelName { get; set; }

		[Required(ErrorMessage = "Starting price is required.")]
		[Column(TypeName = "decimal(18, 2)")]
		[Range(0, 999999.99, ErrorMessage = "Price must be between ₱0 and ₱999,999.99.")]
		[Display(Name = "Starting Price")]
		public decimal StartingPrice { get; set; }

		[Display(Name = "Vehicle Image 1")]
		[StringLength(255)]
		public string? ImagePath { get; set; }



		[Required(ErrorMessage = "Vehicle type is required.")]
		[StringLength(50)]
		[Display(Name = "Vehicle Type")]
		public string VehicleType { get; set; } = "SUV";

		[Required(ErrorMessage = "Status is required.")]
		[StringLength(20)]
		[Display(Name = "Status")]
		public string Status { get; set; } = "Active";

		[Required(ErrorMessage = "Fleet count is required.")]
		[Range(1, 100, ErrorMessage = "Fleet count must be between 1 and 100.")]
		[Display(Name = "Fleet Count")]
		public int FleetCount { get; set; } = 1;

		[Range(0, 100, ErrorMessage = "Available fleet must be between 0 and 100.")]
		[Display(Name = "Available Fleet")]
		public int AvailableFleet { get; set; } = 1;

		[StringLength(50)]
		[Display(Name = "Price Per")]
		public string? PricePer { get; set; } = "per 3 hours";

		[StringLength(500)]
		[Display(Name = "Description")]
		public string? Description { get; set; }

		// Specs & Details
		[StringLength(100)]
		[Display(Name = "Body Type")]
		public string? BodyType { get; set; }

		[StringLength(100)]
		[Display(Name = "Engine Specs")]
		public string? Engine { get; set; }

		[StringLength(100)]
		[Display(Name = "Performance")]
		public string? Performance { get; set; }

		[StringLength(100)]
		[Display(Name = "Transmission")]
		public string? Transmission { get; set; }

		[StringLength(300)]
		[Display(Name = "Variants")]
		public string? Variants { get; set; }

		public virtual ICollection<VehicleBenefit> Benefits { get; set; } = new List<VehicleBenefit>();
		public virtual ICollection<VehicleImage> GalleryImages { get; set; } = new List<VehicleImage>();
	}
}
