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
				.Include(v => v.Benefits)
				.Include(v => v.GalleryImages)
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
				vm.Vehicle.ImagePath = await SaveImageAsync(vm.ImageFile);
				vm.Vehicle.ImagePath = await SaveImageAsync(vm.ImageFile);

				// 1. Add the Vehicle object from the ViewModel to the Context
				_context.Vehicle.Add(vm.Vehicle);

				// 2. Save changes to the database to generate the VehicleId
				await _context.SaveChangesAsync();

				// Process dynamic gallery files
				if (vm.GalleryFiles != null && vm.GalleryFiles.Any())
				{
					foreach (var file in vm.GalleryFiles)
					{
						var savedPath = await SaveImageAsync(file);
						if (!string.IsNullOrEmpty(savedPath))
						{
							_context.VehicleImage.Add(new VehicleImage
							{
								VehicleId = vm.Vehicle.VehicleId,
								ImagePath = savedPath
							});
						}
					}
					await _context.SaveChangesAsync();
				}

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
				.Include(v => v.GalleryImages)
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
					var existingVehicle = await _context.Vehicle.AsNoTracking().FirstOrDefaultAsync(v => v.VehicleId == id);
					if (existingVehicle == null) return NotFound();

					if (vm.ImageFile != null && vm.ImageFile.Length > 0)
					{
						DeleteImage(existingVehicle.ImagePath);
						vm.Vehicle.ImagePath = await SaveImageAsync(vm.ImageFile);
					}
					else { vm.Vehicle.ImagePath = existingVehicle.ImagePath; }

					if (vm.GalleryFiles != null && vm.GalleryFiles.Any())
					{
						// Delete old gallery images
						var oldGallery = _context.VehicleImage.Where(i => i.VehicleId == id).ToList();
						foreach (var oldImage in oldGallery)
						{
							DeleteImage(oldImage.ImagePath);
						}
						_context.VehicleImage.RemoveRange(oldGallery);

						// Save new gallery images
						foreach (var file in vm.GalleryFiles)
						{
							var savedPath = await SaveImageAsync(file);
							if (!string.IsNullOrEmpty(savedPath))
							{
								_context.VehicleImage.Add(new VehicleImage
								{
									VehicleId = id,
									ImagePath = savedPath
								});
							}
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
				.Include(v => v.Benefits) 
				.Include(v => v.GalleryImages)
				.FirstOrDefaultAsync(m => m.VehicleId == id);

		if (vehicle == null) return NotFound();

			return View(vehicle);
		}

		// POST: Vehicles/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicle.Include(v => v.GalleryImages).FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle != null)
            {
                DeleteImage(vehicle.ImagePath);
                foreach (var galleryImg in vehicle.GalleryImages)
                {
                    DeleteImage(galleryImg.ImagePath);
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

		private async Task<string?> SaveImageAsync(IFormFile? imageFile)
		{
			if (imageFile != null && imageFile.Length > 0)
			{
				var uploadsDir = Path.Combine(_env.WebRootPath, "images", "vehicles");
				Directory.CreateDirectory(uploadsDir);
				var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
				var filePath = Path.Combine(uploadsDir, uniqueName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream);
				}
				return uniqueName;
			}
			return null;
		}

		private void DeleteImage(string? imagePath)
		{
			if (!string.IsNullOrEmpty(imagePath))
			{
				var fullPath = Path.Combine(_env.WebRootPath, "images", "vehicles", imagePath);
				if (System.IO.File.Exists(fullPath))
				{
					System.IO.File.Delete(fullPath);
				}
			}
		}
    }
}
