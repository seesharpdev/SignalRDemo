using System.Collections.Generic;
using SignalR.Hubs;
using SignalRDemo.Models;

namespace SignalRDemo.Hubs
{
    public class BooksHub: Hub
    {
        private IBooksRepository _repo = new BooksRepository();

        public IEnumerable<Book> GetBooks()
        {
            var books = _repo.GetBooks();
            return books;
        }

        public void updateBook(Book book)
        {
            var newBook = _repo.UpdateBook(book);
            this.Clients.bookUpdated(newBook);
        }

        public void addBook(Book book)
        {
            var newBook = _repo.AddBook(book);
            this.Clients.bookUpdated(newBook);
        }
    }
}