using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Books;

[Authorize(Roles = "Admin,Librarian")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public DeleteModel(ApplicationDbContext db) => _db = db;

    public Book? Book { get; set; }
    public bool HasBorrowRecords { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Book = await _db.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b => b.BookID == id);
        if (Book == null) return NotFound();
        HasBorrowRecords = await _db.BorrowRecords.AnyAsync(r => r.BookID == id && r.ReturnDate == null);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var book = await _db.Books.Include(b => b.BorrowRecords).FirstOrDefaultAsync(b => b.BookID == id);
        if (book == null) return NotFound();
        if (book.BorrowRecords.Any())
        {
            TempData["Error"] = "Impossible de supprimer: des emprunts existent pour ce livre.";
            return RedirectToPage("/Books/Delete", new { id });
        }
        _db.Books.Remove(book);
        await _db.SaveChangesAsync();
        return RedirectToPage("/Books/Index");
    }
}
