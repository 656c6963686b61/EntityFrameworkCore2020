
namespace ProductShop.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("Product")]
    public class ProductsInRange
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("buyer")]
        public string BuyerFullName { get; set; }
    }
}
