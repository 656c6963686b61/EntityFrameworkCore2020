
namespace CarDealer.Dtos.Import
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Models;

    [XmlType("Car")]
    public class ImportCar
    {
        [XmlIgnore]
        public int Id { get; set; }

        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public List<ImportPartCar> PartCars { get; set; }
    }
}
