using System.ComponentModel.DataAnnotations;

namespace Proel4wProject_EasyRent.Models
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
		[Display(Name = "Email")]
        public string Email { get; set; }

		[Required(ErrorMessage = "Token is required.")]
		public string Token { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		[StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$",
			ErrorMessage = "Password must contain uppercase, lowercase, number, and special character.")]
		[Display(Name = "New Password")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Please confirm your password.")]
		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "The passwords do not match.")]
		[Display(Name = "Confirm Password")]
		public string ConfirmPassword { get; set; }
	}
}
