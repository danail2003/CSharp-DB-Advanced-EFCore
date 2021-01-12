namespace BookShop
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.Collections.Generic;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var titles = context.Books.AsEnumerable()
                .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(x => x.Title)
                .OrderBy(y => y)
                .ToList();

            foreach (var title in titles)
            {
                stringBuilder.AppendLine(title);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var titles = context.Books.AsEnumerable()
                .Where(x => x.Copies < 5000 && x.EditionType.ToString() == "Gold")
                .Select(x => new { x.Title, x.BookId })
                .OrderBy(x => x.BookId)
                .ToList();

            foreach (var title in titles)
            {
                stringBuilder.AppendLine(title.Title);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var titles = context.Books.AsEnumerable()
                .Where(x => x.Price > 40)
                .Select(x => new { x.Title, x.Price })
                .OrderByDescending(x => x.Price)
                .ToList();

            foreach (var title in titles)
            {
                stringBuilder.AppendLine($"{title.Title} - ${title.Price:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var titles = context.Books.AsEnumerable()
                .Where(x => x.ReleaseDate.Value.Year != year)
                .Select(x => new { x.Title, x.BookId })
                .OrderBy(x => x.BookId)
                .ToList();

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var title in titles)
            {
                stringBuilder.AppendLine(title.Title);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> items = input
                .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            List<string> booksTitles = new List<string>();

            foreach (var item in items)
            {
                var books = context.Books
                    .Where(b => b.BookCategories
                    .Any(c => c.Category.Name.ToLower() == item.ToLower()))
                    .Select(b => new
                    {
                        b.Title
                    })
                    .ToList();

                foreach (var book in books)
                {
                    booksTitles.Add(book.Title);
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (var title in booksTitles.OrderBy(x => x))
            {
                sb.AppendLine(title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books.AsEnumerable()
                .Where(x => x.ReleaseDate < dateTime)
                .Select(x => new { x.Title, x.EditionType, x.Price, x.ReleaseDate })
                .OrderByDescending(x => x.ReleaseDate)
                .ToList();

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var book in books)
            {
                stringBuilder.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors.AsEnumerable()
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => new 
                {
                    FullName = x.FirstName + " " + x.LastName
                })
                .ToList();

            StringBuilder stringBuilder = new StringBuilder();

            foreach (var author in authors.OrderBy(x=>x.FullName))
            {
                stringBuilder.AppendLine(author.FullName);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var books = context.Books.AsEnumerable()
                .Where(x => x.Title.ToLower()
                .Contains(input.ToLower()))
                .Select(x => new { x.Title })
                .OrderBy(x => x.Title)
                .ToList();

            foreach (var book in books)
            {
                stringBuilder.AppendLine(book.Title);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new { x.BookId, x.Title, FullName = x.Author.FirstName + " " + x.Author.LastName })
                .OrderBy(x => x.BookId)
                .ToList();

            foreach (var book in books)
            {
                stringBuilder.AppendLine($"{book.Title} ({book.FullName})");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int count = context.Books.Where(x => x.Title.Length > lengthCheck).Count();

            return count;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var copies = context.Authors
                .Select(x => new 
                { 
                    Sum = x.Books.Sum(x => x.Copies),
                    FullName = x.FirstName + " " + x.LastName 
                })
                .OrderByDescending(x => x.Sum)
                .ToList();

            foreach (var author in copies)
            {
                stringBuilder.AppendLine($"{author.FullName} - {author.Sum}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var profits = context.Categories.Select(x => new
            {
                x.Name,
                Profit = x.CategoryBooks.Sum(x=>x.Book.Price * x.Book.Copies)
            })
                .OrderByDescending(x => x.Profit)
                .ThenBy(x => x.Name)
                .ToList();

            foreach (var profit in profits)
            {
                stringBuilder.AppendLine($"{profit.Name} ${profit.Profit:f2}");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var categories = context.Categories.Select(x => new
            {
                x.Name,
                Books = x.CategoryBooks
                .Select(x => new
                {
                    x.Book.Title,
                    x.Book.ReleaseDate,
                    x.Book.ReleaseDate.Value.Year
                })
                .OrderByDescending(x => x.ReleaseDate)
                .Take(3)
                .ToList()
            }).OrderBy(x => x.Name)
            .ToList();

            foreach (var category in categories)
            {
                stringBuilder.AppendLine($"--{category.Name}");

                foreach (var book in category.Books)
                {
                    stringBuilder.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(x => x.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(x => x.Copies < 4200);
            int count = books.Count();

            foreach (var book in books)
            {
                context.Books.Remove(book);
            }

            context.SaveChanges();

            return count;
        }
    }
}
