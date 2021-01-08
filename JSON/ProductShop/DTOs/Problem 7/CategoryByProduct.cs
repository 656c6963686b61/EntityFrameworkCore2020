
namespace ProductShop.DTOs
{
    using Newtonsoft.Json;

    public class CategoryByProduct
    {
        [JsonProperty("category")]
        public string Name { get; set; }

        [JsonProperty("productsCount")]
        public int CategoryProductsCount { get; set; }

        [JsonProperty("averagePrice")]
        public string CategoryProductsAverage { get; set; }

        [JsonProperty("totalRevenue")]
        public string CategoryProductsSum { get; set; }
    }
}
