using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proel4wProject_EasyRent.Data;
using Proel4wProject_EasyRent.Models;
using Proel4wProject_EasyRent.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proel4wProject_EasyRent.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Proel4wProject_EasyRentContext _context;
        private readonly IWebHostEnvironment _env;

        public VehiclesController(Proel4wProject_EasyRentContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

		// GET: Vehicles
		public async Task<IActionResult> Index()
		{
			// Use .Include so the Benefits list isn't empty in the View
			return View(await _context.Vehicle.Include(v => v.Benefits).ToListAsync());
		}

		// GET: Vehicles/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var vehicle = await _context.Vehicle
				.Include(v => v.Benefits) // Load the benefits here too
				.FirstOrDefaultAsync(m => m.VehicleId == id);

			if (vehicle == null) return NotFound();

			return View(vehicle);
		}



		// GET: Vehicles/Create
		[HttpGet]
		public IActionResult Create()
		{
			// Initialize the ViewModel so the View has an object to bind to
			var viewModel = new VehicleCreateViewModel
			{
				// Initialize the Vehicle object to avoid NullReferenceErrors in the View
				Vehicle = new Vehicle(),

				// Pre-fill with empty strings so the View loop has items to render
				BenefitDescriptions = new List<string> { "", "", "", "" }
			};

			return View(viewModel);
		}

		// POST: Vehicles/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(VehicleCreateViewModel vm)
		{
			if (ModelState.IsValid)
			{
				// Handle image upload
				if (vm.ImageFile != null && vm.ImageFile.Length > 0)
				{
					var uploadsDir = Path.Combine(_env.WebRootPath, "images", "vehicles");
					Directory.CreateDirectory(uploadsDir);

					var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
					var filePath = Path.Combine(uploadsDir, uniqueName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await vm.ImageFile.CopyToAsync(stream);
					}

					vm.Vehicle.ImagePath = uniqueName;
				}

				// 1. Add the Vehicle object from the ViewModel to the Context
				_context.Vehicle.Add(vm.Vehicle);

				// 2. Save changes to the database to generate the VehicleId
				await _context.SaveChangesAsync();

				// 3. Map each string in the BenefitDescriptions list to the VehicleBenefit model
				if (vm.BenefitDescriptions != null)
				{
					foreach (var desc in vm.BenefitDescriptions.Where(d => !string.IsNullOrEmpty(d)))
					{
						_context.VehicleBenefit.Add(new VehicleBenefit
						{
							Description = desc,
							VehicleId = vm.Vehicle.VehicleId // Use the ID generated in step 2
						});
					}
					await _context.SaveChangesAsync();
				}

				return RedirectToAction(nameof(Index));
			}

			// If validation fails, return the ViewModel back to the View
			return View(vm);
		}

		// GET: Vehicles/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var vehicle = await _context.Vehicle
				.Include(v => v.Benefits)
				.FirstOrDefaultAsync(m => m.VehicleId == id);

			if (vehicle == null) return NotFound();

			var viewModel = new VehicleCreateViewModel
			{
				Vehicle = vehicle,
				BenefitDescriptions = vehicle.Benefits.Select(b => b.Description).ToList()
			};

			return View(viewModel);
		}

		// POST: Vehicles/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, VehicleCreateViewModel vm)
		{
			if (id != vm.Vehicle.VehicleId) return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					// Handle image upload
					if (vm.ImageFile != null && vm.ImageFile.Length > 0)
					{
						var uploadsDir = Path.Combine(_env.WebRootPath, "images", "vehicles");
						Directory.CreateDirectory(uploadsDir);

						// Delete old image if exists
						var existingVehicle = await _context.Vehicle.AsNoTracking().FirstOrDefaultAsync(v => v.VehicleId == id);
						if (existingVehicle != null && !string.IsNullOrEmpty(existingVehicle.ImagePath))
						{
							var oldPath = Path.Combine(uploadsDir, existingVehicle.ImagePath);
							if (System.IO.File.Exists(oldPath))
							{
								System.IO.File.Delete(oldPath);
							}
						}

						var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
						var filePath = Path.Combine(uploadsDir, uniqueName);

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await vm.ImageFile.CopyToAsync(stream);
						}

						vm.Vehicle.ImagePath = uniqueName;
					}
					else
					{
						// Keep the existing image path if no new file uploaded
						var existingVehicle = await _context.Vehicle.AsNoTracking().FirstOrDefaultAsync(v => v.VehicleId == id);
						if (existingVehicle != null)
						{
							vm.Vehicle.ImagePath = existingVehicle.ImagePath;
						}
					}

					_context.Update(vm.Vehicle);
					await _context.SaveChangesAsync();

					var oldBenefits = _context.VehicleBenefit.Where(b => b.VehicleId == id);
					_context.VehicleBenefit.RemoveRange(oldBenefits);

					if (vm.BenefitDescriptions != null)
					{
						foreach (var desc in vm.BenefitDescriptions.Where(d => !string.IsNullOrEmpty(d)))
						{
							_context.VehicleBenefit.Add(new VehicleBenefit
							{
								Description = desc,
								VehicleId = id
							});
						}
					}
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!VehicleExists(vm.Vehicle.VehicleId)) return NotFound();
					else throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(vm);
		}

		// GET: Vehicles/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var vehicle = await _context.Vehicle
				.Include(v => v.Benefits) // Include benefits so they show on the confirmation page
				.FirstOrDefaultAsync(m => m.VehicleId == id);

		if (vehicle == null) return NotFound();

			return View(vehicle);
		}

		// POST: Vehicles/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(vehicle.ImagePath))
                {
                    var imagePath = Path.Combine(_env.WebRootPath, "images", "vehicles", vehicle.ImagePath);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Vehicle.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

		

		private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.VehicleId == id);
        }
    }
}
