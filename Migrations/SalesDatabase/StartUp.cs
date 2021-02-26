namespace P03_SalesDatabase
{
    using P03_SalesDatabase.Data;

    class StartUp
    {
        static void Main()
        {
            SalesContext db = new SalesContext();
            db.Database.EnsureCreated();
        }
    }
}
