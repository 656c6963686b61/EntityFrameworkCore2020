namespace BookShop
{
    using System;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Data;
    using Models;
    using Models.Enums;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            Console.WriteLine(RemoveBooks(db));
           
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

            return output.TrimEnd();
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

        //problem 6
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context
                .Books
                .Select(x => new
                {
                    x.Title,
                    x.ReleaseDate,
                    x.EditionType,
                    x.Price
                })
                .Where(x => x.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(x => x.ReleaseDate)
                .Take(10)
                .ToList();

            string output = string.Empty;

            foreach (var book in books)
            {
                output += $"{book.Title} - {book.EditionType} - ${book.Price}" + Environment.NewLine;
            }

            return output.TrimEnd();
        }

        //problem 7
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName
                })
                .Where(x => x.FirstName.ToLower().EndsWith(input.ToLower()))
                .OrderBy(x => x.FirstName)
                .ToList();

            string output = string.Empty;
            foreach (var author in authors)
            {
                output += author.FirstName + " " + author.LastName + Environment.NewLine;
            }
            return output.TrimEnd();
        }

        //problem 8
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Select(x => x.Title)
                .Where(x => x.ToLower().Contains(input.ToLower()))
                .OrderBy(x => x)
                .ToList();

            return string.Join(Environment.NewLine,books);
        }

        //problem 9
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Select(x => new
                {
                    AuthorFirstName = x.Author.FirstName,
                    AuthorLastName = x.Author.LastName,
                    x.BookId,
                    x.Title,
                })
                .Where(x => x.AuthorLastName.ToLower().StartsWith(input))
                .OrderBy(x => x.BookId)
                .ToList();

            string output = string.Empty;

            foreach (var book in books)
            {
                output += $"{book.Title} ({book.AuthorFirstName} {book.AuthorLastName})" + Environment.NewLine;
            }

            return output.TrimEnd();
        }

        //problem 10
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context
                .Books
                .Count(x => x.Title.Length > lengthCheck);
        }

        //problem 11
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context
                .Authors
                .Select(x => new
                {
                    FullName = x.FirstName + " " + x.LastName,
                    Books = x.Books.Sum(x => x.Copies)
                })
                .OrderByDescending(x => x.Books)
                .ToList();

            string output = string.Empty;

            foreach (var author in authors)
            {
                output += $"{author.FullName} - {author.Books}" + Environment.NewLine;
            }

            return output.TrimEnd();
        }

        //problem 12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(x => new
                {
                    Profit = x.CategoryBooks.Sum(x => x.Book.Price * x.Book.Copies),
                    x.Name
                })
                .OrderByDescending(x => x.Profit)
                .ThenBy(x => x.Name)
                .ToList();

            string output = string.Empty;

            foreach (var category in categories)
            {
                output += $"{category.Name} ${category.Profit:f2}" + Environment.NewLine;
            }

            return output.TrimEnd();
        }

        //problem 13
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(x => new
                {
                    Books = x.CategoryBooks
                        .OrderByDescending(x => x.Book.ReleaseDate)
                        .Take(3)
                        .Select(x => new
                        {
                            x.Book.Title,
                            x.Book.ReleaseDate
                        })
                        .ToList(),
                    x.Name
                })
                .OrderBy(x => x.Name)
                .ToList();

            string output = string.Empty;

            foreach (var category in categories)
            {
                output += $"--{category.Name}" + Environment.NewLine;
                foreach (var book in category.Books)
                {
                    output += $"{book.Title} ({book.ReleaseDate.Value.Year})" + Environment.NewLine;
                }
                output.TrimEnd();
            }

            return output;
        }
        
        //problem 14
        public static void IncreasePrices(BookShopContext context)
        {
            var booksToUpdate = context
                .Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in booksToUpdate)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //problem 15
        public static int RemoveBooks(BookShopContext context)
        {
            var booksToDelete = context
                .Books
                .Where(x => x.Copies < 4200)
                .ToList();

            int counter = 0;
            foreach (var book in booksToDelete)
            {
                context.Books.Remove(book);
                counter++;
            }

            context.SaveChanges();
            return counter;
        }
    }
}
