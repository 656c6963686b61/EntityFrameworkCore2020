
namespace CarDealer.Dtos.Import
{
    using System.Xml.Serialization;
    using Models;

    [XmlType("partId")]
    public class ImportPartCar
    {
        [XmlAttribute("id")]
        public int PartId { get; set; }

        [XmlIgnore]
        public Car Car { get; set; }

        [XmlIgnore]
        public int CarId { get; set; }
    }
}