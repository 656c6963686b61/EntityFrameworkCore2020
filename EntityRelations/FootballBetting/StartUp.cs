using FootballBetting.Data;

namespace FootballBetting
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new FootballBettingContext();
            db.Database.EnsureCreated();
        }
    }
}
