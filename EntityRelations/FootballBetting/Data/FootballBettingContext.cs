using System.Runtime.CompilerServices;
using FootballBetting.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {
            
        }

        public FootballBettingContext(DbContextOptions<FootballBettingContext> options)
            :base(options)
        {
               
        }

        public DbSet<User> Users { get; set; } 
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=FootballBetting;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasOne(x => x.AwayTeam)
                .WithMany(x => x.AwayGames)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(x => x.HomeTeam)
                .WithMany(x => x.HomeGames)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasOne(x => x.PrimaryKitColor)
                .WithMany(x => x.PrimaryKitTeams)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasOne(x => x.SecondaryKitColor)
                .WithMany(x => x.SecondaryKitTeams)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<PlayerStatistic>()
                .HasKey(x => new {x.GameId, x.PlayerId});
        }
    }
}
