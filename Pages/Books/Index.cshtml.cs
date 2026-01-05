using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Books;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IndexModel(ApplicationDbContext db) => _db = db;

    public IList<Book> Books { get; set; } = new List<Book>();

    public string? Q { get; set; }

    public async Task OnGetAsync(string? q)
    {
        Q = q;
        IQueryable<Book> query = _db.Books.Include(b => b.Author).Include(b => b.Category);
        if (!string.IsNullOrWhiteSpace(Q))
        {
            query = query.Where(b =>
                b.Title.Contains(Q) ||
                b.ISBN.Contains(Q) ||
                (b.Author != null && (b.Author.FirstName.Contains(Q) || b.Author.LastName.Contains(Q))) ||
                (b.Category != null && b.Category.Name.Contains(Q)));
        }
        Books = await query.OrderBy(b => b.Title).ToListAsync();
    }
}
