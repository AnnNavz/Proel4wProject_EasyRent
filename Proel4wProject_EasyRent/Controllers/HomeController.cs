using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proel4wProject_EasyRent.Data;
using Proel4wProject_EasyRent.Models;
using Microsoft.AspNetCore.Http;
using Proel4wProject_EasyRent.Extensions;

namespace Proel4wProject_EasyRent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Proel4wProject_EasyRentContext _context;

        public HomeController(ILogger<HomeController> logger, Proel4wProject_EasyRentContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Home/Vehicles
        public async Task<IActionResult> Vehicles(
            string? vehicleType,
            string? status,
            decimal? minPrice,
            decimal? maxPrice,
            string? searchQuery,
            int page = 1)
        {
            int pageSize = 6;

            var query = _context.Vehicle
                .Include(v => v.Benefits)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(vehicleType) && vehicleType != "All")
            {
                query = query.Where(v => v.VehicleType == vehicleType);
            }

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(v => v.Status == status);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(v => v.StartingPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(v => v.StartingPrice <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(v => v.ModelName.Contains(searchQuery) ||
                                         (v.Description != null && v.Description.Contains(searchQuery)));
            }

            // Get total count before pagination
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Ensure page is within valid range
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var vehicles = await query
                .OrderBy(v => v.ModelName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get distinct vehicle types for filter dropdown
            var vehicleTypes = await _context.Vehicle
                .Select(v => v.VehicleType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            // Pass data to view
            ViewBag.VehicleTypes = vehicleTypes;
            ViewBag.CurrentType = vehicleType;
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSearch = searchQuery;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(vehicles);
        }

        // GET: Home/VehicleDetails/5
        public async Task<IActionResult> VehicleDetails(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicle
                .Include(v => v.Benefits)
                .Include(v => v.GalleryImages)
                .FirstOrDefaultAsync(m => m.VehicleId == id);

            if (vehicle == null) return NotFound();

            return View(vehicle);
        }

        // GET: Home/ReservationDetails/5
        public async Task<IActionResult> ReservationDetails(int? id)
        {
            if (id == null) return NotFound();

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.VehicleId == id);

            if (vehicle == null) return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    ViewBag.UserFirstName = user.UserFirstName;
                    ViewBag.UserLastName = user.UserLastName;
                    ViewBag.UserEmail = user.UserEmail;
                }
            }

            return View(vehicle);
        }

        // POST: Home/SubmitReservationDetails
        [HttpPost]
        public async Task<IActionResult> SubmitReservationDetails(ReservationSessionModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("LoginView", "Account");
            model.UserId = userId.Value;

            // Fetch Vehicle to calculate pricing
            var vehicle = await _context.Vehicle.FindAsync(model.VehicleId);
            if (vehicle != null)
            {
                model.VehicleName = vehicle.ModelName;
                model.VehicleType = vehicle.VehicleType;
                model.BasePrice = vehicle.StartingPrice;
                model.PricePer = vehicle.PricePer;

                var duration = model.ReturnDate.Add(model.ReturnTime) - model.PickupDate.Add(model.PickupTime);
                double totalHours = Math.Ceiling(duration.TotalHours);
                if (totalHours <= 0) totalHours = 1;

                model.TimeAccumulatedDisplay = duration.Days > 0 
                    ? $"{duration.Days} days and {duration.Hours} hours" 
                    : $"{totalHours} hours";
                
                // Calculate pricing based mathematically on absolute 3-Hour blocks
                int totalBlocks = (int)Math.Ceiling(totalHours / 3.0);
                if (totalBlocks <= 0) totalBlocks = 1;

                model.TotalPartialFees = vehicle.StartingPrice; // Cost of the absolute first block

                if (totalBlocks > 1)
                {
                    int succeedingBlocks = totalBlocks - 1;
                    model.SucceedingFee = succeedingBlocks * vehicle.StartingPrice; // Every single overflow fraction incurs a new massive base fee charge
                }
                else
                {
                    model.SucceedingFee = 0;
                }
                
                // If Personal Use -> 5% Downpayment. If Company -> Total payments without extra.
                if (model.UsageType == "Personal Use")
                {
                    model.Downpayment = (model.TotalPartialFees + model.SucceedingFee) * 0.05m;
                    model.TotalFeeToBePaid = model.Downpayment;
                }
                else
                {
                    model.Downpayment = 0;
                    model.TotalFeeToBePaid = model.TotalPartialFees + model.SucceedingFee;
                }
                model.FinalTotalCost = model.TotalPartialFees + model.SucceedingFee;
            }

            // Save to Session using JSON Extension
            HttpContext.Session.SetObject("ActiveReservation", model);

            return RedirectToAction("PaymentDetails");
        }

        // GET: Home/PaymentDetails
        public IActionResult PaymentDetails()
        {
            var model = HttpContext.Session.GetObject<ReservationSessionModel>("ActiveReservation");
            if (model == null) return RedirectToAction("Vehicles");

            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitPaymentDetails(string PaymentMethod)
        {
            var model = HttpContext.Session.GetObject<ReservationSessionModel>("ActiveReservation");
            if (model == null) return RedirectToAction("Vehicles");

            model.PaymentMethod = PaymentMethod;
            HttpContext.Session.SetObject("ActiveReservation", model);

            return RedirectToAction("PaymentConfirmation");
        }

        // GET: Home/PaymentConfirmation
        public IActionResult PaymentConfirmation()
        {
            var model = HttpContext.Session.GetObject<ReservationSessionModel>("ActiveReservation");
            if (model == null) return RedirectToAction("Vehicles");

            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitPaymentConfirmation(string AccountName, string ReferenceNumber, decimal AmountSent)
        {
            var model = HttpContext.Session.GetObject<ReservationSessionModel>("ActiveReservation");
            if (model == null) return RedirectToAction("Vehicles");

            model.AccountName = AccountName;
            model.ReferenceNumber = ReferenceNumber;
            model.AmountSent = AmountSent;

            HttpContext.Session.SetObject("ActiveReservation", model);
            return RedirectToAction("BookingConfirmation");
        }

        // GET: Home/BookingConfirmation
        public IActionResult BookingConfirmation()
        {
            var model = HttpContext.Session.GetObject<ReservationSessionModel>("ActiveReservation");
            if (model == null) return RedirectToAction("Vehicles");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizeBooking()
        {
            var model = HttpContext.Session.GetObject<ReservationSessionModel>("ActiveReservation");
            if (model == null) return RedirectToAction("Vehicles");

            var reservation = new Reservation
            {
                UserId = model.UserId,
                VehicleId = model.VehicleId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                EmailAddress = model.EmailAddress,
                PassengerCapacity = model.PassengerCapacity,
                PickupDate = model.PickupDate,
                PickupTime = model.PickupTime,
                ReturnDate = model.ReturnDate,
                ReturnTime = model.ReturnTime,
                UsageType = model.UsageType,
                PickupAddress = model.PickupAddress,
                DestinationAddress = model.DestinationAddress,
                TotalPartialFees = model.TotalPartialFees,
                SucceedingFee = model.SucceedingFee,
                Downpayment = model.Downpayment,
                FinalTotalCost = model.FinalTotalCost,
                PaymentMethod = model.PaymentMethod,
                PaymentAccountName = model.AccountName,
                PaymentReferenceNumber = model.ReferenceNumber,
                PaymentAmountSent = model.AmountSent,
                Status = "Pending Verification",
                DateCreated = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Clear session mapping now that it's physically in the database
            HttpContext.Session.Remove("ActiveReservation");

            TempData["Message"] = "Reservation successfully booked! Waiting for Admin verification.";
            return RedirectToAction("MyBookings"); 
        }

        // GET: Home/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("LoginView", "Account");

            var bookings = await _context.Reservations
                .Include(r => r.Vehicle)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            return View(bookings);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
