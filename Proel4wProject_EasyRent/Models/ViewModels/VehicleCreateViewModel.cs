using Microsoft.AspNetCore.Http;

namespace Proel4wProject_EasyRent.Models.ViewModels
{
	public class VehicleCreateViewModel
	{
		public Vehicle Vehicle { get; set; }

		// This holds the benefits the user types in
		public List<string> BenefitDescriptions { get; set; } = new List<string>();

		// Image file upload
		public IFormFile? ImageFile { get; set; }
	}
}
