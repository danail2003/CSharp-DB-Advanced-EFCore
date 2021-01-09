using System;

namespace P01_StudentSystem
{
    using P01_StudentSystem.Data;

    class StartUp
    {
        static void Main()
        {
            StudentSystemContext db = new StudentSystemContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
