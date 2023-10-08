using Project1.Models;
namespace Project1.DTO
{
    public class OrderDTO
    {
        public int BikeId { get; set; }
        public int Total { get; set; }
        public string ShipmentType { get; set; }
        public string ShipmentDestination { get; set; }
    }
}
