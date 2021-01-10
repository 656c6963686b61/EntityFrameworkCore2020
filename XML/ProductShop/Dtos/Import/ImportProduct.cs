namespace ProductShop.Dtos.Import
{
    using System.Xml.Serialization;

    [XmlType("Product")]
    public class ImportProduct
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("sellerId")]
        public int SellerId { get; set; }

        [XmlElement("buyerId")]
        public int BuyerId { get; set; }
    }
}
