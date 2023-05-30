using System;

namespace eRental.Models
{
    public class Rental
    {
        public int RentalId { get; set; }
        public string UserId { get; set; }
        public int VehicleId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal TotalCost { get; set; }

        public decimal CalculateRentalCost()
        {
            // Implement your logic to calculate the rental cost based on your business rules
            // You can access the RentalDate, ReturnDate, and other properties to calculate the cost
            // You may consider the rental duration, vehicle type, pricing strategy, etc.
            decimal rentalCost = 0.0m;
            // Calculate the rental cost logic goes here
            return rentalCost;
        }
    }
}