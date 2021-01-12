namespace CarDealer.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class PartCar
    {
        public int PartId { get; set; }
        public Part Part { get; set; }
        
        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}
