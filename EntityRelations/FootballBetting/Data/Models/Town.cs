using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Data.Models
{
    public class Town
    {
        public Town()
        {
            this.Teams = new List<Team>();
        }
        [Key]
        public int TownId { get; set; }
        
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        
        public Country Country { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public ICollection<Team> Teams { get; set; }
    }
}
