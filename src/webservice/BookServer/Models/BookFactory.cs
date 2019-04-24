using System;
using System.Collections.Generic;
using System.Linq;

namespace BookService.Models
{
    public static class BookFactory
    {
        public static Dictionary<string, Tuple<DateTime,List<Book>>> Books = new Dictionary<string, Tuple<DateTime,List<Book>>>();

        public static void Initialize(string authorizationToken)
        {
            Books.Add(authorizationToken, 
                Tuple.Create(DateTime.Now.AddHours(60), DefaultBooks.ToList()));
        }
        private static IEnumerable<Book> DefaultBooks
        {
            get
            {
                yield return new Book
                {
                    ISBN = "0545685192",
                    Title = "Minecraft: The Complete Handbook Collection",
                    Authors = new List<string> { "Stephanie Milton", "Paul Soares Jr.", "Jordan Maron", "Nick Farwell", },
                    PublishDate = new DateTime(2014, 10, 1),
                    Genre = "Games",
                };
                yield return new Book
                {
                    ISBN = "0553801473",
                    Title = "A Dance with Dragons (A Song of Ice and Fire, Book 5)",
                    Authors = new List<string> { "George R.R. Martin" },
                    PublishDate = new DateTime(2011, 07, 12),
                    Genre = "Fantasy",
                };
                yield return new Book
                {
                    ISBN = "0544272994",
                    Title = "What If?: Serious Scientific Answers to Absurd Hypothetical Questions",
                    Authors = new List<string> { "Randall Munroe" },
                    PublishDate = new DateTime(2014, 9, 4),
                    Genre = "Nonfiction",
                };
                yield return new Book
                {
                    ISBN = "141971189X",
                    Title = "Diary of a Wimpy Kid: The Long Haul",
                    Authors = new List<string> { "Jeff Kinney" },
                    PublishDate = new DateTime(2014, 11, 10),
                    Genre = "Fiction",
                };
                yield return new Book
                {
                    ISBN = "1256324778",
                    Title = "Gray Mountain",
                    Authors = new List<string> { "John Grisham" },
                    PublishDate = new DateTime(2014, 10, 21),
                    Genre = "Thriller",
                };
            }
        }

        public static void ClearStaleData()
        {
            var keys = Books.Keys.ToList();
            foreach (var oneKey in keys)
            {
                Tuple<DateTime, List<Book>> result;
                if (Books.TryGetValue(oneKey, out result))
                {
                    if (result.Item1 < DateTime.Now)
                    {
                        Books.Remove(oneKey);
                    }
                }
            }
        }

        static readonly Random Rng = new Random();
        public static string CreateISBN()
        {
            char[] ch = new char[10];
            for (int i = 0; i < 10; i++)
            {
                ch[i] = (char)('0' + Rng.Next(0, 9));
            }
            return new string(ch);
        }
    }
}
