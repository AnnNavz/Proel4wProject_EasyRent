using System.ComponentModel.DataAnnotations;

namespace Proel4wProject_EasyRent.Models
{
	public class ResetPasswordViewModel
	{
		[Required]
        public string Email { get; set; }

		[Required]
		public string Token { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		[StringLength(100, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$",
			ErrorMessage = "Password must contain uppercase, lowercase, number, and special character.")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "The passwords do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
