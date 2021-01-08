namespace ProductShop.DTOs.Problem_8
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class ProductsDTO
    {
        [JsonProperty("products")]
        public ICollection<ProductDTO> ProductsSold { get; set; }

        [JsonProperty("count")]
        public int ProductsSoldCount { get; set; }
    }
}
