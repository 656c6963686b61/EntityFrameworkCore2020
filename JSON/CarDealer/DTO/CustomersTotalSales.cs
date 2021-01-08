namespace CarDealer.DTO
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;

    public class CustomersTotalSales
    {
        [JsonProperty("fullName")]
        public string Name { get; set; }

        [JsonProperty("boughtCars")]
        public int BoughtCars { get; set; }

        [JsonProperty("spentMoney")]
        public decimal SpentMoney { get; set; }
    }
}
