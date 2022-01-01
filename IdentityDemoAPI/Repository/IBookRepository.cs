using IdentityDemoAPI.Data;
using IdentityDemoAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityDemoAPI.Repository
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBookAsync();
        Task<(int, string)> InsertNewBook(BookInsertModel model);
        Task<(int, string)> DeleteBook(Guid bookId);
        Task<(int, string)> EditBook(BookUpdateModel model);
    }
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext context;
        public BookRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBookAsync()
        {
            return await context.Books.Where(b => b.Status).Include(b => b.BookCategories).ThenInclude(bc => bc.Category).AsNoTracking().ToListAsync();
        }

        public async Task<(int, string)> InsertNewBook(BookInsertModel model)
        {
            (int code, string message) = (-100, "Insert failed - Duplicate ISBN");
            var book = await context.Books.Where(b => b.ISBN == model.ISBN).AsNoTracking().FirstOrDefaultAsync();
            if (book == null)
            {
                Guid bookId = Guid.NewGuid();
                int categoryCheck = -1;
                List<BookCategory> bookCategories = new();
                foreach (int cateId in model.BookCategories)
                {
                    var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cateId);
                    if (category == null)
                    {
                        categoryCheck = cateId;
                        break;
                    }
                    bookCategories.Add(new BookCategory { BookId = bookId, CategoryId = cateId });
                }
                if (categoryCheck != -1)
                {
                    code = -102;
                    message = $"Insert failed - Category Id {categoryCheck} not found";
                } 
                else
                {
                    book = new Book
                    {
                        Id = bookId,
                        Title = model.Title,
                        ISBN = model.ISBN,
                        Author = model.Author,
                        Publisher = model.Publisher,
                        PublishedDate = model.PublishedDate,
                        Price = model.Price,
                        TotalQuantity = model.TotalQuantity,
                        ImageUrl = model.ImageUrl,
                        Status = true,
                        BookCategories = bookCategories
                    };
                    await context.Books.AddAsync(book);
                    code = 0;
                    message = "Insert Successfully";
                }
            }
            return (code, message);
        }

        public async Task<(int, string)> EditBook(BookUpdateModel model)
        {
            Guid bookId = model.BookId;
            (int code, string message) = (-201, $"Edit failed - Book Id {bookId} not found");
            var book = await context.Books.FindAsync(bookId);
            if (book != null)
            {
                int categoryCheck = -1;
                if (model.BookCategories != null)
                {
                    List<BookCategory> newBookCategories = new();
                    foreach (int cateId in model.BookCategories)
                    {
                        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == cateId);
                        if (category == null)
                        {
                            categoryCheck = cateId;
                            break;
                        }
                        newBookCategories.Add(new BookCategory { BookId = bookId, CategoryId = cateId });
                    }
                    if (categoryCheck == -1)
                    {
                        var contextBookCategories = context.BookCategories;
                        var oldCategories = await contextBookCategories.Where(bc => bc.BookId == bookId).ToArrayAsync();
                        contextBookCategories.RemoveRange(oldCategories);

                        await contextBookCategories.AddRangeAsync(newBookCategories);
                    }
                    else
                    {
                        return (-202, $"Edit failed - Category Id {categoryCheck} not found");
                    }
                }
                book.Author = model.Author;
                book.Price = model.Price;
                book.PublishedDate = model.PublishedDate;
                book.Publisher = model.Publisher;
                book.Title = model.Title;
                book.TotalQuantity = model.TotalQuantity;
                if (model.ImageUrl != null) book.ImageUrl = model.ImageUrl;
                code = 0;
                message = "Edit Successfully";
            }
            return (code, message);
        }

        public async Task<(int, string)> DeleteBook(Guid bookId)
        {
            (int code, string message) = (-301, $"Delete failed - Book Id {bookId} not found");
            var book = await context.Books.FindAsync(bookId);
            if (book != null)
            {
                book.Status = false;
                code = 0;
                message = "Remove Successfully";
            }
            return (code, message);
        }
    }
}
