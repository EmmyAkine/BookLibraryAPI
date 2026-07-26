using BookLibraryAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPI.Data {
    public class LibraryDbContext : IdentityDbContext {

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }
        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);
            builder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Clean Code", Author = "Robert Martin", ISBN = "9780132350884", CopiesAvailable = 3 },
                new Book { Id = 2, Title = "The Pragmatic Programmer", Author = "Andy Hunt", ISBN = "9780135957059", CopiesAvailable = 2 },
                new Book { Id = 3, Title = "Design Patterns", Author = "Erich Gamma", ISBN = "9780201633610", CopiesAvailable = 1 },
                new Book { Id = 4, Title = "Refactoring", Author = "Martin Fowler", ISBN = "9780134757599", CopiesAvailable = 4 },
                new Book { Id = 5, Title = "Domain-Driven Design", Author = "Eric Evans", ISBN = "9780321125217", CopiesAvailable = 2 }
            );
        }
    }
}
