using System.Collections.Generic;

using SignalR.Hubs;

using SignalRDemo.Models;

namespace SignalRDemo.Hubs
{
    public class BooksHub: Hub
    {
        private readonly IBooksRepository _repository = new BooksRepository();

        //public BooksHub(IBooksRepository repository)
        //{
        //    _repository = repository;
        //}

        public IEnumerable<Book> GetBooks()
        {
            var books = _repository.GetBooks();
            return books;
        }

        public void updateBook(Book book)
        {
            var newBook = _repository.UpdateBook(book);
            Clients.bookUpdated(newBook);
        }

        public void addBook(Book book)
        {
            var newBook = _repository.AddBook(book);
            Clients.bookUpdated(newBook);
        }
    }
}