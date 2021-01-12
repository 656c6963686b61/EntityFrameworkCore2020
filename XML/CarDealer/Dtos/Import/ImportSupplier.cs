namespace CarDealer
{
    using System.Xml.Serialization;

    [XmlType("Supplier")]
    public class ImportSupplier
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("isImporter")]
        public bool IsImporter { get; set; }
    }
}
