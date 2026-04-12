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
                    HttpContext.Session.SetString("UserName", $"{user.UserFirstName} {user.UserLastName}");
                    if (!string.IsNullOrEmpty(user.ProfileImagePath))
                    {
                        HttpContext.Session.SetString("UserProfileImage", user.ProfileImagePath);
                    }

                    
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

		// --- PROFILE & SAVED VEHICLES FEATURES ---

		public async Task<IActionResult> Profile()
		{
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("LoginView");

            var user = await _context.Users.FindAsync(userId);
            return View(user);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string UserFirstName, string UserLastName, string UserEmail, IFormFile? ProfileImage)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("LoginView");

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.UserFirstName = UserFirstName;
                user.UserLastName = UserLastName;
                user.UserEmail = UserEmail;

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ProfileImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ProfileImage.CopyToAsync(stream);
                    }
                    user.ProfileImagePath = uniqueFileName;
                }

                _context.Update(user);
                await _context.SaveChangesAsync();
                
                // Update Session
                HttpContext.Session.SetString("UserName", $"{user.UserFirstName} {user.UserLastName}");
                if (!string.IsNullOrEmpty(user.ProfileImagePath))
                    HttpContext.Session.SetString("UserProfileImage", user.ProfileImagePath);
                
                TempData["Message"] = "Profile successfully updated!";
            }

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> SavedVehicles()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("LoginView");

            var savedVehicles = await _context.SavedVehicles
                .Include(s => s.Vehicle)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SavedDate)
                .ToListAsync();

            return View(savedVehicles);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleSavedVehicle(int vehicleId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Not logged in" });

            var existing = await _context.SavedVehicles.FirstOrDefaultAsync(s => s.UserId == userId && s.VehicleId == vehicleId);
            bool isSaved = false;
            
            if (existing != null)
            {
                _context.SavedVehicles.Remove(existing);
            }
            else
            {
                _context.SavedVehicles.Add(new SavedVehicle { UserId = userId.Value, VehicleId = vehicleId });
                isSaved = true;
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true, isSaved = isSaved });
        }
    }
}
