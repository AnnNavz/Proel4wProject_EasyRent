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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedInput = HashingServices.HashData(model.Password);

                // Include the Role navigation property if you need to check Role Name, 
                // otherwise checking RoleId directly is faster.
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserEmail == model.Email && u.Password == hashedInput);

                if (user != null)
                {
                    // Optional: Store user info in Session
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetInt32("UserRole", user.RoleId);

                    // Redirect based on RoleId
                    return user.RoleId switch
                    {
                        1 => RedirectToAction("Index", "Users"),
                        2 => RedirectToAction("Index", "Home"),
                        3 => RedirectToAction("Index", "Home"),
                        _ => RedirectToAction("Index", "Home") // Default fallback
                    };
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }
    }
}
