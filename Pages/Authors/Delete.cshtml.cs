using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Authors;

[Authorize(Roles = "Admin,Librarian")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _db;
    
    public DeleteModel(ApplicationDbContext db) => _db = db;

    [BindProperty]
    public Author? Author { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Author = await _db.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.AuthorID == id);
        
        if (Author == null)
        {
            return NotFound();
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Author == null || Author.AuthorID == 0)
        {
            return NotFound();
        }

        var authorToDelete = await _db.Authors.FirstOrDefaultAsync(a => a.AuthorID == Author.AuthorID);
        
        if (authorToDelete == null)
        {
            return NotFound();
        }

        _db.Authors.Remove(authorToDelete);
        await _db.SaveChangesAsync();
        
        return RedirectToPage("/Authors/Index");
    }
}