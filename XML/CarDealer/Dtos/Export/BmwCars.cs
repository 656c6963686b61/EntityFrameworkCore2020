namespace CarDealer.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("car")]
    public class BmwCars
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TraveledDistance { get; set; }
    }
}
