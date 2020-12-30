using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Data.Models
{
    public class PlayerStatistic
    {
        [ForeignKey(nameof(Player))]
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }
        public Game Game { get; set; }

        [Required]
        public int Assists { get; set; }
        
        public int MinutesPlayed { get; set; }
        
        [Required]
        public int ScoredGoals { get; set; }
    }
}
