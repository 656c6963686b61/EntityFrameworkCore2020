namespace CarDealer.Dtos.Export
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Models;

    [XmlType("car")]
    public class CarsWithDistance
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("travelled-distance")]
        public long TraveledDistance { get; set; }

        [XmlIgnore]
        public List<PartCar> PartCars { get; set; }
    }
}
