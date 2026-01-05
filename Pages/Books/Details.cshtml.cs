using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Books;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public DetailsModel(ApplicationDbContext db) => _db = db;

    public Book? Book { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Book = await _db.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b => b.BookID == id);
        if (Book == null) return NotFound();
        return Page();
    }
}
