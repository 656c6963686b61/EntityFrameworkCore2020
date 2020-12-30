using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace FootballBetting.Data.Models
{
    public class Team
    {
        public Team()
        {
            this.AwayGames = new List<Game>();
            this.HomeGames = new List<Game>();
            this.Players = new List<Player>();
        }
        
        [Key]
        public int TeamId { get; set; }
        
        [Required]
        public decimal Budget { get; set; }
        
        public Initials Initials { get; set; }
        
        public string LogoUrl { get; set; }

        [Required]
        public string Name { get; set; }
        

        [ForeignKey(nameof(PrimaryKitColor))]
        public int PrimaryKitColorId { get; set; }
        public Color PrimaryKitColor { get; set; }

        
        [ForeignKey(nameof(SecondaryKitColor))]
        public int SecondaryKitColorId { get; set; }
        public Color SecondaryKitColor { get; set; }
        
        
        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }
        public Town Town { get; set; }
        

        [InverseProperty("HomeTeam")]
        public ICollection<Game> HomeGames { get; set; }

        [InverseProperty("AwayTeam")]
        public ICollection<Game> AwayGames { get; set; }
        
        public ICollection<Player> Players { get; set; }
    }
}
