using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proel4wProject_EasyRent.Data;
using Proel4wProject_EasyRent.Models;
using Proel4wProject_EasyRent.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proel4wProject_EasyRent.Controllers
{
    public class UsersController : Controller
    {
        private readonly Proel4wProject_EasyRentContext _context;

        public UsersController(Proel4wProject_EasyRentContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

		// GET: Users/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var user = await _context.Users
				.Include(u => u.Role) // CRITICAL: Loads the Role table data
				.FirstOrDefaultAsync(m => m.UserId == id);

			if (user == null) return NotFound();

			return View(user);
		}

		// GET: Users/Create
		public IActionResult Create()
        {
            ViewData["UserRole"] = new SelectList(_context.Role, "RoleId", "UserRole");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserFirstName,UserLastName,UserEmail,Password,ConfirmPassword,RoleId")] Users users)
        {
            string HashPassword = HashingServices.HashData(users.Password);
            users.Password = HashPassword;
            users.ConfirmPassword = HashPassword;

            if (ModelState.IsValid)
            {
                _context.Add(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserRole"] = new SelectList(_context.Role, "RoleId", "UserRole", users.RoleId);
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            ViewData["UserRole"] = new SelectList(_context.Role, "RoleId", "UserRole", users.RoleId);
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserFirstName,UserLastName,UserEmail,Password,ConfirmPassword,RoleId")] Users users)
        {
            if (id != users.UserId)
            {
                return NotFound();
            }

            string HashPassword = HashingServices.HashData(users.Password);
            users.Password = HashPassword;
            users.ConfirmPassword = HashPassword; // Set both to the hashed value

            if (ModelState.IsValid)
            {
                try
                {
                    // FIX: Use Update instead of Add to prevent primary key violations
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserRole"] = new SelectList(_context.Role, "RoleId", "UserRole", users.RoleId);
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users != null)
            {
                _context.Users.Remove(users);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
