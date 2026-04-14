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

        public async Task<IActionResult> Dashboard()
        {
            // Fetch real data for dashboard stats
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.ActiveRentals = await _context.Reservations
                .CountAsync(r => r.Status == "Pending Verification" || r.Status == "Confirmed");
            
            var totalFleet = await _context.Vehicle.CountAsync();
            var availableFleet = await _context.Vehicle.SumAsync(v => v.AvailableFleet);
            var totalFleetCount = await _context.Vehicle.SumAsync(v => v.FleetCount);
            ViewBag.FleetPercentage = totalFleetCount > 0 
                ? Math.Round((double)availableFleet / totalFleetCount * 100) 
                : 0;

            // Fetch recent users for the activity table
            ViewBag.RecentUsers = await _context.Users
                .OrderByDescending(u => u.UserId)
                .Take(5)
                .ToListAsync();

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

        // POST: Admin/UpdateReservationStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReservationStatus(int reservationId, string newStatus)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation != null)
            {
                reservation.Status = newStatus;
                _context.Update(reservation);
                await _context.SaveChangesAsync();
                TempData["Message"] = $"Reservation #{reservationId} status updated to {newStatus}.";
            }
            return RedirectToAction("Reservations");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginView", "Account");
        }
    }
}
