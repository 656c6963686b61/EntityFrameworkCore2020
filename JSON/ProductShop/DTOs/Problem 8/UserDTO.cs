namespace ProductShop.DTOs.Problem_8
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UserDTO
    {
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        //[JsonProperty("soldProducts")]
        //public ProductsDTO SoldProducts { get; set; }

        [JsonProperty("count")]
        public int ProductsSoldCount { get; set; }

        [JsonProperty("soldProducts")]
        public ICollection<ProductDTO> ProductsSold { get; set; }
    }
}
