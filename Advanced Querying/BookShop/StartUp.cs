namespace BookShop
{
    using System;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Text;
    using Data;
    using Models.Enums;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            var ageRestriction = Console.ReadLine();
            Console.WriteLine(GetBooksByAgeRestriction(db, ageRestriction));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            //age restriction is stored as a number in the database
            //to string doesn't work

            AgeRestriction ageRestriction = 0;

            if (command.ToLower() == "minor")
            {
                ageRestriction = AgeRestriction.Minor;
            }
            else if (command.ToLower() == "teen")
            {
                ageRestriction = AgeRestriction.Teen;
            }
            else if (command.ToLower() == "adult")
            {
                ageRestriction = AgeRestriction.Adult;
            }

            var result = context
                .Books
                .Where(x => x.AgeRestriction == ageRestriction)
                .Select(x => x.Title)
                .OrderBy(x => x)
                .Take(10)
                .ToList();
            return string.Join("\n", result);
        }
    }
}
