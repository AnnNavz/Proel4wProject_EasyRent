using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proel4wProject_EasyRent.Data;

namespace Proel4wProject_EasyRent.Controllers
{
    public class AdminController : Controller
    {
        private readonly Proel4wProject_EasyRentContext _context;

        public AdminController(Proel4wProject_EasyRentContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Reservations()
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            return View(reservations);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginView", "Account");
        }
    }
}
