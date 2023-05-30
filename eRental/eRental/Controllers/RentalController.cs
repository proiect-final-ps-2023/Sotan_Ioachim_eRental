using eRental.Data;
using eRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;

namespace eRental.Controllers
{
    public class RentalController : Controller
    {
        public readonly MyDbContext _context;
        public readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public RentalController(MyDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public IActionResult SerializeData()
        {
            // Retrieve the rental records from the database
            var rentals = _context.Rental.ToList();

            // Create a new XML serializer
            var serializer = new XmlSerializer(typeof(List<Rental>));

            // Generate a unique file name for the XML file
            var fileName = $"Rentals_{DateTime.Now:yyyyMMddHHmmss}.xml";

            // Combine the file name with the server's physical path
            var filePath = Path.Combine(_env.WebRootPath, fileName);

            try
            {
                // Create a new file stream to write the XML data
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // Serialize the rental records to the file stream
                    serializer.Serialize(fileStream, rentals);
                }

                // Return a success message or perform any other necessary actions
                Console.WriteLine("Serialization complete.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during serialization
                // Log or display the error message as needed
                Console.WriteLine($"Serialization error: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

    }
}

