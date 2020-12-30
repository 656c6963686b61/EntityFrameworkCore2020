using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Data.Models
{
    public class Bet
    {
        [Key]
        public int BetId { get; set; }
        
        [Required]
        public decimal Amount { get; set; }

        [ForeignKey(nameof(Game))]
        public int GameId { get; set; }

        [Required]
        public Game Game { get; set; }

        [Required]
        public string Prediction { get; set; }
        
        [Required]
        public DateTime DateTime { get; set; }
        
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; }
    }
}
