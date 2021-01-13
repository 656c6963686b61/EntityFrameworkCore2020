namespace CarDealer.Dtos.Export
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("car")]
    public class CarCAP
    {
        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public List<PartCAP> Parts { get; set; }
    }
}
