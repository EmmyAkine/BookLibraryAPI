using BookLibraryAPI.Data;
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

        [HttpGet]
        public IActionResult GetAll() {
            var books = _context.Books.ToList();
            return Ok(books);
        }

    }
}
