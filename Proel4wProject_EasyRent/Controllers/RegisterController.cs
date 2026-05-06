using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proel4wProject_EasyRent.Data;
using Proel4wProject_EasyRent.Models;
using Proel4wProject_EasyRent.Services;

namespace Proel4wProject_EasyRent.Controllers
{
	public class RegisterController : Controller
	{
		private readonly Proel4wProject_EasyRentContext _context;

		public RegisterController(Proel4wProject_EasyRentContext context)
		{
			_context = context;
		}

		// GET: Register/Register
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		// Added ConfirmPassword to the Bind attribute
		public async Task<IActionResult> Register([Bind("UserFirstName,UserLastName,UserEmail,Password,ConfirmPassword")] Users users)
		{
			// We assign these manually, so don't let them cause validation errors
			ModelState.Remove("RoleId");
			ModelState.Remove("Role");

			if (ModelState.IsValid)
			{
				// Check if email already exists
				var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == users.UserEmail);
				if (existingUser != null)
				{
					ModelState.AddModelError("UserEmail", "Email address is already registered.");
					return View(users);
				}

				// 1. Hash the password
				string hash = HashingServices.HashData(users.Password);
				users.Password = hash;
				users.ConfirmPassword = hash; // Set both to the hashed value

				// 2. Automatically assign RoleId 2 (Customer)
				users.RoleId = 2;

				_context.Add(users);
				await _context.SaveChangesAsync();

				// Set success message and redirect to login page
				TempData["Message"] = "Registration successful! Please sign in with your new account.";
				return RedirectToAction("LoginView", "Account");
			}
			// If we got this far, something failed; redisplay form with errors
			return View(users);
		}
	}
}
