using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace gestion_biblio.Pages.Books;

[Authorize(Roles = "Admin,Librarian")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public EditModel(ApplicationDbContext db) => _db = db;

    [BindProperty]
    public Book Book { get; set; } = new();

    public IEnumerable<SelectListItem> AuthorOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> CategoryOptions { get; set; } = Enumerable.Empty<SelectListItem>();

    [BindProperty]
    public IFormFile? uploadedFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Book = await _db.Books.FirstOrDefaultAsync(b => b.BookID == id) ?? new Book();
        if (Book.BookID == 0) return NotFound();

        AuthorOptions = await _db.Authors
            .OrderBy(a => a.LastName)
            .Select(a => new SelectListItem { Value = a.AuthorID.ToString(), Text = $"{a.FirstName} {a.LastName}" })
            .ToListAsync();
        CategoryOptions = await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.CategoryID.ToString(), Text = c.Name })
            .ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var existing = await _db.Books.FirstOrDefaultAsync(b => b.BookID == Book.BookID);
        if (existing == null) return NotFound();

        existing.Title = Book.Title;
        existing.AuthorID = Book.AuthorID;
        existing.ISBN = Book.ISBN;
        existing.PublicationYear = Book.PublicationYear;
        existing.CategoryID = Book.CategoryID;
        existing.Genre = Book.Genre;
        existing.Description = Book.Description;
        existing.TotalCopies = Book.TotalCopies;
        // Keep AvailableCopies within range
        existing.AvailableCopies = Math.Min(Math.Max(Book.AvailableCopies, 0), existing.TotalCopies);
if (uploadedFile != null)
{
    Directory.CreateDirectory(Path.Combine("wwwroot", "images"));
    var fileName = Guid.NewGuid() + Path.GetExtension(uploadedFile.FileName);
    var filePath = Path.Combine("wwwroot/images", fileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await uploadedFile.CopyToAsync(stream);
    }

    existing.PicturePath = fileName;
}

        await _db.SaveChangesAsync();
        return RedirectToPage("/Books/Index");
    }
}
