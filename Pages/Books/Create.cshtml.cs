using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Books;

[Authorize(Roles = "Admin,Librarian")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public CreateModel(ApplicationDbContext db) => _db = db;

    [BindProperty]
    public Book Book { get; set; } = new();

    public IEnumerable<SelectListItem> AuthorOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> CategoryOptions { get; set; } = Enumerable.Empty<SelectListItem>();

    public async Task OnGetAsync()
    {
        AuthorOptions = await _db.Authors
            .OrderBy(a => a.LastName)
            .Select(a => new SelectListItem { Value = a.AuthorID.ToString(), Text = $"{a.FirstName} {a.LastName}" })
            .ToListAsync();
        CategoryOptions = await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.CategoryID.ToString(), Text = c.Name })
            .ToListAsync();
    }

   public async Task<IActionResult> OnPostAsync(IFormFile? uploadedFile)
{
    if (!ModelState.IsValid)
    {
        await OnGetAsync();
        return Page();
    }

    if (Book.AvailableCopies == 0)
    {
        Book.AvailableCopies = Book.TotalCopies;
    }

    if (uploadedFile != null)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(uploadedFile.FileName);
        var filePath = Path.Combine("wwwroot/images", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await uploadedFile.CopyToAsync(stream);
        }

        Book.PicturePath = fileName;
    }

    _db.Books.Add(Book);
    await _db.SaveChangesAsync();
    return RedirectToPage("/Books/Index");
}

}
