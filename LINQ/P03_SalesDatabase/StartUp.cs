using System;

namespace P03_SalesDatabase
{
    using System.Collections.Generic;
    using Data;
    using Data.Models;
    using Microsoft.EntityFrameworkCore;
    using RandomDataGenerator.FieldOptions;
    using RandomDataGenerator.Randomizers;

    class StartUp
    {
        static void Main(string[] args)
        {
            var db = new SalesContext();
            db.Database.EnsureCreated();

        }
    }
}
