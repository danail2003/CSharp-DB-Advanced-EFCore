namespace P03_FootballBetting
{
    using P03_FootballBetting.Data;

    class StartUp
    {
        static void Main()
        {
            FootballBettingContext db = new FootballBettingContext();
            db.Database.EnsureCreated();
        }
    }
}
