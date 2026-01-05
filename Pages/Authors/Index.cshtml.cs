using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Authors;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IndexModel(ApplicationDbContext db) => _db = db;

    public IList<Author> Authors { get; set; } = new List<Author>();

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    public async Task OnGetAsync()
    {
        IQueryable<Author> query = _db.Authors;
        if (!string.IsNullOrWhiteSpace(Q))
        {
            query = query.Where(a => a.FirstName.Contains(Q) || a.LastName.Contains(Q));
        }
        Authors = await query.OrderBy(a => a.LastName).ToListAsync();
    }
}