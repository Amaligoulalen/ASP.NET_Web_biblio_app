using gestion_biblio.Data;
using gestion_biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace gestion_biblio.Pages.Members;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IndexModel(ApplicationDbContext db) => _db = db;

    public IList<Member> Members { get; set; } = new List<Member>();
    public string? Q { get; set; }

    public async Task OnGetAsync(string? q)
    {
        Q = q;
        IQueryable<Member> query = _db.Members;
        if (!string.IsNullOrWhiteSpace(Q))
        {
            query = query.Where(m => m.FirstName.Contains(Q) || m.LastName.Contains(Q) || m.Email.Contains(Q));
        }
        Members = await query.OrderBy(m => m.LastName).ToListAsync();
    }
}
