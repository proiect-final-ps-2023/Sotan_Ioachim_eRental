using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eRental.Data;
using eRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Drawing.Printing;
using NuGet.Protocol.Plugins;

namespace eRental.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public VehiclesController(MyDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
              return _context.Vehicle != null ? 
                          View(await _context.Vehicle.ToListAsync()) :
                          Problem("Entity set 'MyDbContext.Vehicle'  is null.");
        }

        public async Task<IActionResult> AdminIndex()
        {
            return _context.Vehicle != null ?
                          View(await _context.Vehicle.ToListAsync()) :
                          Problem("Entity set 'MyDbContext.Vehicle'  is null.");
        }

        // GET: Vehicles/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // POST: Vehicles/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                return RedirectToAction("AdminIndex", await _context.Vehicle.Where(j => j.Make.Contains(SearchPhrase)).ToListAsync());
            return RedirectToAction("Index", await _context.Vehicle.Where(j => j.Make.Contains(SearchPhrase)).ToListAsync());

        }

        // GET: Vehicles/OrderByPrice
        public async Task<IActionResult> OrderByPrice(String orderByPrice)
        {
            var vehicles = _context.Vehicle.AsQueryable(); ;
            if (orderByPrice == "Descending")
            {
                vehicles = vehicles.OrderByDescending(v => v.Price);
            }
            else
            {
                vehicles = vehicles
                    .OrderBy(v => v.Price);
            }

            var user = _userManager.GetUserAsync(User).Result;
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                return RedirectToAction("AdminIndex",vehicles);
            return RedirectToAction("Index",vehicles);
          
        }


        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,Make,Model,Year,Type,Price")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int?id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,Make,Model,Year,Type,Price")] Vehicle vehicle)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId))
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
            return View(vehicle);
        }


        // GET: Vehicles/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.VehicleId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Vehicle == null)
            {
                return Problem("Entity set 'MyDbContext.Vehicle'  is null.");
            }
            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicle.Remove(vehicle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
          return (_context.Vehicle?.Any(e => e.VehicleId == id)).GetValueOrDefault();
        }

        //POST: Vehicles/Rent/5
        public async Task<IActionResult> Rent(int? id)
        {
            if (id == null || _context.Vehicle == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.VehicleId == id);

            if ( vehicle == null) 
            { 
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> Rent(int vehicleId, DateTime rentalDate, DateTime returnDate)
        {
            Console.WriteLine(vehicleId);
            // Get the currently logged-in user
            var user = _userManager.GetUserAsync(User).Result;
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            TimeSpan duration = returnDate.Subtract(rentalDate);
            int numberOfDays = duration.Days;

            var vehicle = _context.Find<Vehicle>(vehicleId);

            // Create a new rental record
            var rental = new Rental
            {
                UserId = user.Id,
                VehicleId = vehicleId,
                RentalDate = rentalDate,
                ReturnDate = returnDate,
                TotalCost = (decimal) (numberOfDays * vehicle.Price) 
            };

            // Save the rental record to the database
            _context.Rental.Add(rental);
            _context.SaveChanges();

            // Redirect to a success page or perform any other necessary actions
            
            if(isAdmin)
                return RedirectToAction("AdminIndex");
            return RedirectToAction("Index");
        }

    }
}
