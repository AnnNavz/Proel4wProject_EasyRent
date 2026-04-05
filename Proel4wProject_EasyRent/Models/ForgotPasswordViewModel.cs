using System.ComponentModel.DataAnnotations;

namespace Proel4wProject_EasyRent.Models
{
	public class ForgotPasswordViewModel
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress]
		public string Email { get; set; }
	}
}
