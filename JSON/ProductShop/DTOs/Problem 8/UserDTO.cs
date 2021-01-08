namespace ProductShop.DTOs.Problem_8
{
    using Newtonsoft.Json;

    public class UserDTO
    {
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("soldProducts")]
        public ProductsDTO ProductsSold { get; set; }
    }
}
