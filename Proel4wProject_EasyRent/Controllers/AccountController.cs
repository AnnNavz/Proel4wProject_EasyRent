using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proel4wProject_EasyRent.Data;
using Proel4wProject_EasyRent.Models;
using Proel4wProject_EasyRent.Services;

namespace Proel4wProject_EasyRent.Controllers
{
    public class AccountController : Controller
    {
        private readonly Proel4wProject_EasyRentContext _context;

        public AccountController(Proel4wProject_EasyRentContext context)
        {
            _context = context;
        }

        public IActionResult LoginView()
        {
            return View();
        }

		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _context.Users
					.FirstOrDefaultAsync(u => u.UserEmail == model.Email);

				if (user != null)
				{
					return RedirectToAction("ResetPassword", new { email = model.Email, token = "internal-reset" });
				}

				ModelState.AddModelError("", "Email not found in our system.");
			}
			return View(model);
		}

		public IActionResult ResetPassword(string email, string token)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
			{
				return RedirectToAction("LoginView");
			}

			var model = new ResetPasswordViewModel { Email = email, Token = token };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _context.Users
					.FirstOrDefaultAsync(u => u.UserEmail == model.Email);

				if (user != null)
				{
					string hashedNewPassword = HashingServices.HashData(model.NewPassword);

					user.Password = hashedNewPassword;

					user.ConfirmPassword = hashedNewPassword;

					_context.Update(user);
					await _context.SaveChangesAsync();

					TempData["Message"] = "Your password has been reset successfully. Please log in.";
					return RedirectToAction("LoginView");
				}

				ModelState.AddModelError("", "User not found.");
			}
			return View(model);
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedInput = HashingServices.HashData(model.Password);

               
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserEmail == model.Email && u.Password == hashedInput);

                if (user != null)
                {
                    
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetInt32("UserRole", user.RoleId);

                    
                    return user.RoleId switch
                    {
                        1 => RedirectToAction("Dashboard", "Admin"),
                        2 => RedirectToAction("Index", "Home"),
                        3 => RedirectToAction("Index", "Home"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }
    }
}
