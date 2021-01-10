namespace ProductShop.Dtos.Export
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("User")]
    public class UsersSoldProducts
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public List<ProductsUsersSoldProducts> SoldProducts { get; set; }
    }
}
