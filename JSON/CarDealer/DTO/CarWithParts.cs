namespace CarDealer.DTO
{
    using System.Collections.Generic;
    using Models;
    using Newtonsoft.Json;

    public class CarWithParts
    {
        [JsonProperty("car")]
        public CarFromCarsAndParts Car { get; set; }

        [JsonProperty("parts")]
        public ICollection<PartFromCarsAndParts> Parts { get; set; }
    }
}
