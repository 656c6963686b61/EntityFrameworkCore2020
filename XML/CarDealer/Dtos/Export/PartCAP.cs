namespace CarDealer.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("parts")]
    public class PartCAP
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
