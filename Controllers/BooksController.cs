using BookLibraryAPI.Data;
using BookLibraryAPI.DTO;
using BookLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext dbContext) {
            _context = dbContext;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetAllBooks() {
            var books = _context.Books.ToList();
            return Ok(books);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetBookById(int id) {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) {
                return NotFound();
            }
            return Ok(book);
        }


        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadBook(UploadBookDto dto) {
            var book = new Book {
                Title = dto.Title,
                Author = dto.Author,
                ISBN = dto.ISBN,
                CopiesAvailable = dto.CopiesAvailable
            };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBook(int id) {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) {
                return NotFound();
            }
            _context.Books.Remove(book);
            _context.SaveChanges();
            return Ok("Book deleted.");
        }

    }
}
