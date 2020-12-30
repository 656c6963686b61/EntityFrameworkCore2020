using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballBetting.Data.Models
{
    public class User
    {
        public User()
        {
            this.Bets = new List<Bet>();
        }
        
        [Key]
        public int UserId { get; set; }

        [Required]
        public decimal Balance { get; set; }
        
        [Required]
        [Column(TypeName = "varchar(30)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string Name { get; set; }
        
        [Required]
        [Column(TypeName = "varchar(30)")]
        public string Password { get; set; }
        
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string Username { get; set; }
        
        public ICollection<Bet> Bets { get; set; }
    }
}
