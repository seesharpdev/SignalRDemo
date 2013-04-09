using System;
using System.Collections.Generic;

namespace SignalRDemo.Models
{
    public interface IBooksRepository : IDisposable
    {
        List<Book> GetBooks();
        Book GetBook(int id);
        Book AddBook(Book book);
        Book UpdateBook(Book book);
        void DeleteBook(int id);
    }
}