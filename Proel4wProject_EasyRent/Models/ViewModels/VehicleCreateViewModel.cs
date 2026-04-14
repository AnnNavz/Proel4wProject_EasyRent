using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proel4wProject_EasyRent.Models.ViewModels
{
	public class VehicleCreateViewModel
	{
		[Required(ErrorMessage = "Vehicle information is required.")]
		[Display(Name = "Vehicle")]
		public Vehicle Vehicle { get; set; }

		// This holds the benefits the user types in
		[Display(Name = "Benefits")]
		public List<string> BenefitDescriptions { get; set; } = new List<string>();

		// Image file upload
		[Display(Name = "Main Image")]
		public IFormFile? ImageFile { get; set; }

		// Dynamic gallery files upload
		[Display(Name = "Gallery Images")]
		public List<IFormFile> GalleryFiles { get; set; } = new List<IFormFile>();
	}
}
