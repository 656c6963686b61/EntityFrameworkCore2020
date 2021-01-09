namespace ProductShop.DTOs.Problem_8
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Models;

    public class ProductsDTO
    {

        [JsonProperty("count")]
        public int ProductsSoldCount { get; set; }

        [JsonProperty("products")]
        public ICollection<ProductDTO> ProductsSold { get; set; }
    }
}
