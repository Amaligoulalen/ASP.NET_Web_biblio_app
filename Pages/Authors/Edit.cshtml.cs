using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Authors;

[Authorize(Roles = "Admin,Librarian")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;
    
    public EditModel(ApplicationDbContext db) => _db = db;

    [BindProperty]
    public Author Author { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Author = await _db.Authors.FirstOrDefaultAsync(a => a.AuthorID == id) ?? new Author();
        
        if (Author.AuthorID == 0)
        {
            return NotFound();
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingAuthor = await _db.Authors.FirstOrDefaultAsync(a => a.AuthorID == Author.AuthorID);
        
        if (existingAuthor == null)
        {
            return NotFound();
        }

        existingAuthor.FirstName = Author.FirstName;
        existingAuthor.LastName = Author.LastName;

        await _db.SaveChangesAsync();
        
        return RedirectToPage("/Authors/Index");
    }
}