namespace ProductShop.DTOs.Problem_8
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UsersDTO
    {
        public UsersDTO()
        {
            this.UsersCount = Users.Count;
        }

        [JsonProperty("usersCount")]
        public int UsersCount { get; set; }
        
        [JsonProperty("users")]
        public ICollection<UsersDTO> Users { get; set; }
    }
}
