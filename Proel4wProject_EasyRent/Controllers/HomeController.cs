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
            decimal? minPrice,
            decimal? maxPrice,
            string? searchQuery,
            int page = 1)
        {
            int pageSize = 6;

            // Only show Active vehicles to customers
            var query = _context.Vehicle
                .Include(v => v.Benefits)
                .Where(v => v.Status == "Active")
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(vehicleType) && vehicleType != "All")
            {
                query = query.Where(v => v.VehicleType == vehicleType);
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

            // Get distinct vehicle types for filter dropdown (from active vehicles)
            var vehicleTypes = await _context.Vehicle
                .Where(v => v.Status == "Active")
                .Select(v => v.VehicleType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            // Compute available units for today for each vehicle
            var today = DateTime.Today;
            var vehicleIds = vehicles.Select(v => v.VehicleId).ToList();
            var todayBookings = await _context.Reservations
                .Where(r => vehicleIds.Contains(r.VehicleId)
                    && (r.Status == "Confirmed" || r.Status == "Pending Verification")
                    && r.PickupDate <= today && r.ReturnDate >= today)
                .GroupBy(r => r.VehicleId)
                .Select(g => new { VehicleId = g.Key, BookedCount = g.Count() })
                .ToListAsync();

            var availableToday = new Dictionary<int, int>();
            foreach (var v in vehicles)
            {
                var booked = todayBookings.FirstOrDefault(b => b.VehicleId == v.VehicleId)?.BookedCount ?? 0;
                availableToday[v.VehicleId] = Math.Max(0, v.FleetCount - booked);
            }

            // Pass data to view
            ViewBag.VehicleTypes = vehicleTypes;
            ViewBag.CurrentType = vehicleType;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSearch = searchQuery;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.AvailableToday = availableToday;

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

            // Check if fully booked today
            var today = DateTime.Today;
            var bookedToday = await _context.Reservations
                .CountAsync(r => r.VehicleId == id
                    && (r.Status == "Confirmed" || r.Status == "Pending Verification")
                    && r.PickupDate <= today && r.ReturnDate >= today);

            ViewBag.IsFullyBookedToday = bookedToday >= vehicle.FleetCount;
            ViewBag.AvailableUnitsToday = Math.Max(0, vehicle.FleetCount - bookedToday);

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

            // Get booked dates for the next 90 days for this vehicle
            var today = DateTime.Today;
            var endDate = today.AddDays(90);
            var reservations = await _context.Reservations
                .Where(r => r.VehicleId == id
                    && (r.Status == "Confirmed" || r.Status == "Pending Verification")
                    && r.ReturnDate >= today && r.PickupDate <= endDate)
                .Select(r => new { r.PickupDate, r.ReturnDate })
                .ToListAsync();

            // Build a dictionary of date -> booked unit count
            var fullyBookedDates = new List<string>();
            for (var date = today; date <= endDate; date = date.AddDays(1))
            {
                var bookedCount = reservations.Count(r => r.PickupDate <= date && r.ReturnDate >= date);
                if (bookedCount >= vehicle.FleetCount)
                {
                    fullyBookedDates.Add(date.ToString("yyyy-MM-dd"));
                }
            }

            ViewBag.FullyBookedDatesJson = System.Text.Json.JsonSerializer.Serialize(fullyBookedDates);
            ViewBag.FleetCount = vehicle.FleetCount;

            return View(vehicle);
        }

        // JSON API: Get booked dates for a vehicle
        [HttpGet]
        public async Task<IActionResult> GetBookedDates(int vehicleId)
        {
            var vehicle = await _context.Vehicle.FindAsync(vehicleId);
            if (vehicle == null) return NotFound();

            var today = DateTime.Today;
            var endDate = today.AddDays(90);
            var reservations = await _context.Reservations
                .Where(r => r.VehicleId == vehicleId
                    && (r.Status == "Confirmed" || r.Status == "Pending Verification")
                    && r.ReturnDate >= today && r.PickupDate <= endDate)
                .Select(r => new { r.PickupDate, r.ReturnDate })
                .ToListAsync();

            var fullyBookedDates = new List<string>();
            for (var date = today; date <= endDate; date = date.AddDays(1))
            {
                var bookedCount = reservations.Count(r => r.PickupDate <= date && r.ReturnDate >= date);
                if (bookedCount >= vehicle.FleetCount)
                {
                    fullyBookedDates.Add(date.ToString("yyyy-MM-dd"));
                }
            }

            return Json(new { bookedDates = fullyBookedDates, fleetCount = vehicle.FleetCount });
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
                // Server-side validation: check if dates overlap with fully booked dates
                var overlappingBookings = await _context.Reservations
                    .CountAsync(r => r.VehicleId == model.VehicleId
                        && (r.Status == "Confirmed" || r.Status == "Pending Verification")
                        && r.PickupDate <= model.ReturnDate && r.ReturnDate >= model.PickupDate);

                if (overlappingBookings >= vehicle.FleetCount)
                {
                    TempData["Error"] = "Sorry, this vehicle is fully booked for your selected dates. Please choose different dates.";
                    return RedirectToAction("ReservationDetails", new { id = model.VehicleId });
                }

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
                    model.SucceedingFee = succeedingBlocks * vehicle.StartingPrice;
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
