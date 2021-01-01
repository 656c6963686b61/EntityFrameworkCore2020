using System;

namespace P03_SalesDatabase
{
    using Data;

    class StartUp
    {
        static void Main(string[] args)
        {
            var db = new SalesContext();
            db.Database.EnsureCreated();
        }
    }
}
