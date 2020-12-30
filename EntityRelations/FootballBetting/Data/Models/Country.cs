using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Data.Models
{
    public class Country
    {
        public Country()
        {
            this.Towns = new List<Town>();
        }
        [Key]
        public int CountryId { get; set; }

        [Required]
        public string Name { get; set; }
        
        public ICollection<Town> Towns { get; set; }
    }
}
