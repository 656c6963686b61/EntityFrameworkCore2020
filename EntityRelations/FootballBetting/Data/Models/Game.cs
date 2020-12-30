using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FootballBetting.Data.Models
{
    public class Game
    {
        public Game()
        {
            this.Bets = new List<Bet>();
            this.PlayerStatistic = new List<PlayerStatistic>();
        }
        
        [Key]
        public int GameId { get; set; }
        
        [Required]
        public double AwayTeamBetRate { get; set; }
        
        [Required]
        public int AwayTeamGoals { get; set; }

        [Required]
        [ForeignKey(nameof(AwayTeam))]
        public int AwayTeamId { get; set; }
        
        public Team AwayTeam { get; set; }

        [Required]
        public double DrawBetRate { get; set; }

        [Required]
        public double HomeTeamBetRate { get; set; }

        [Required]
        public int HomeTeamGoals { get; set; }

        [Required]
        [ForeignKey(nameof(HomeTeam))]
        public int HomeTeamId { get; set; }
        
        public Team HomeTeam { get; set; }

        [Required]
        public string Result { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public ICollection<Bet> Bets { get; set; }
        
        public ICollection<PlayerStatistic> PlayerStatistic { get; set; }
    }
}
