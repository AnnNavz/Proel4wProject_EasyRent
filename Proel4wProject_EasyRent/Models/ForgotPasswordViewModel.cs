using System.ComponentModel.DataAnnotations;

namespace Proel4wProject_EasyRent.Models
{
	public class ForgotPasswordViewModel
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		[StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
		[Display(Name = "Email Address")]
		public string Email { get; set; }
	}
}
