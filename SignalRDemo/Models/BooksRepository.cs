using System.Collections.Generic;
using System.Linq;
using System;

namespace SignalRDemo.Models
{ 
    public class BooksRepository : IBooksRepository
    {
        private static readonly List<Book> Books;

        static BooksRepository()
        {
            Books = new List<Book>
            {
                new Book
                    {
                    Id = 1,
                    Title = "The Ultimate Hitchhiker's Guide to the Galaxy",
                    Author = "Douglas Adams",
                    ImageName = "TheUltimateHitchhikersGuide.jpg",
                },
                new Book
                {
                    Id = 2,
                    Title = "2001: A Space Odyssey",
                    Author = "Arthur C. Clarke",
                    ImageName = "2001ASpaceOdyssey.jpg",
                },
                new Book
                {
                    Id = 3,
                    Title = "The War of the Worlds",
                    Author = "H G Wells",
                    ImageName = "TheWaroftheWorlds.jpg",
                },
                new Book
                {
                    Id = 4,
                    Title = "Jurassic Park",
                    Author = "Michael Crichton",
                    ImageName = "JurassicPark.jpg",
                },
                new Book
                {
                    Id = 5,
                    Title = "I, Robot",
                    Author = "Isaac Asimov",
                    ImageName = "IRobot.jpg",
                },
            };
        }

        public List<Book> GetBooks()
        {
            return Books.OrderBy(b => b.Id).ToList();
        }

        public Book GetBook(int id)
        {
            return Books.SingleOrDefault(b => b.Id == id);
        }

        public void Dispose()
        {
        }

        public Book AddBook(Book book)
        {
            book.Id = Environment.TickCount;
            Books.Add(book);

            return book;
        }

        public Book UpdateBook(Book book)
        {
            var persistedBook = Books.Single(b => b.Id == book.Id);
            Books.Remove(persistedBook);
            Books.Add(book);

            return book;
        }


        public void DeleteBook(int id)
        {
            var persistedBook = Books.SingleOrDefault(b => b.Id == id);
            if (persistedBook != null)
            {
                Books.Remove(persistedBook);
            }
        }
    }
}