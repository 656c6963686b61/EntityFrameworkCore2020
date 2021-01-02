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
            Console.WriteLine(GetBooksByCategory(db, "horror mystery drama"));
        }

        //problem 1
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
                .ToList();
            return string.Join("\n", result);
        }

        //problem 2
        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context
                .Books
                .Where(x => x.Copies < 5000 && x.EditionType == EditionType.Gold)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            return string.Join("\n", goldenBooks);
        }

        //problem 3
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context
                .Books
                .Where(x => x.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();

            string output = string.Empty;

            foreach (var book in books)
            {
                output += $"{book.Title} - ${book.Price}" + "\n";
            }

            return output;
        }

        //problem 4
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context
                .Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            return string.Join("\n", books);
        }

        //problem 5
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.Split().Select(x => x.ToLower()).ToList();
            var books = context
                .Books
                .Where(x => x.BookCategories
                    .Any(y => categories.Contains(y.Category.Name.ToLower())))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();
            
            return string.Join(Environment.NewLine, books);
        }
    }
}
