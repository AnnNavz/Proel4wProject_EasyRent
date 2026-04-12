using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proel4wProject_EasyRent.Data;
using Proel4wProject_EasyRent.Models;

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

        public IActionResult Privacy()
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
