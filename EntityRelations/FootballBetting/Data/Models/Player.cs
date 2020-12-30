using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Data.Models
{
    public class Player
    {
        public Player()
        {
            this.PlayerStatistics = new List<PlayerStatistic>();
        }
        [Key]
        public int PlayerId { get; set; }   
        
        [Required]
        public bool IsInjured { get; set; }

        [Required]
        public string Name { get; set; }
        
        
        [ForeignKey(nameof(Position))]
        public int PositionId { get; set; }
        [Required]
        public Position Position { get; set; }

        [Required]
        public int SquadNumber { get; set; }
        
        [ForeignKey(nameof(Team))]
        public int TeamId { get; set; }
        [Required]
        public Team Team { get; set; }
        
        public ICollection<PlayerStatistic> PlayerStatistics { get; set; }
    }
}
