
namespace ProductShop.Dtos.Import
{
    using System.Xml.Serialization;

    [XmlType("Category")]
    public class ImportCategory
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
