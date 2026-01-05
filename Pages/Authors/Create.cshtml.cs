using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace gestion_biblio.Pages.Authors;

[Authorize(Roles = "Admin,Librarian")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    
    public CreateModel(ApplicationDbContext db) => _db = db;

    [BindProperty]
    public Author Author { get; set; } = new();

    public void OnGet()
    {
        // Initialize page
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _db.Authors.Add(Author);
        await _db.SaveChangesAsync();
        
        return RedirectToPage("/Authors/Index");
    }
}