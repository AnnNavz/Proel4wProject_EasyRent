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
                // 1. Hash the incoming password using your HashingService
                string hashedInput = HashingServices.HashData(model.Password);

                // 2. Check database for a user with this email and hashed password
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserEmail == model.Email && u.Password == hashedInput);

                if (user != null)
                {
                    // Logic for signing in (e.g., setting a Cookie or Session)
                    // HttpContext.Session.SetString("UserName", user.UserFirstName);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }
    }
}
